using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Models;
using AsvAvaloniaToDoSample.Pages.ToDoList.RoutedEvents;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

public class ToDoItemViewModel : RoutableViewModel
{
    public const string PageId = "todo-item";
    private readonly ReactiveProperty<bool> _isChecked;

    private readonly string _itemId;

    public ToDoItemViewModel()
        : this(
            new ToDoItem("Hello", false),
            new ReactiveCommand<ToDoItemViewModel>(),
            NullLoggerFactory.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    public ToDoItemViewModel(
        ToDoItem toDoItem,
        ReactiveCommand<ToDoItemViewModel> removeItem,
        ILoggerFactory loggerFactory)
        : base(new NavigationId(PageId, toDoItem.Id), loggerFactory)
    {
        _itemId = toDoItem.Id;

        RemoveItem = removeItem;
        Content = new BindableReactiveProperty<string>(toDoItem.Content).DisposeItWith(Disposable);

        _isChecked = new ReactiveProperty<bool>(toDoItem.IsChecked).DisposeItWith(Disposable);
        IsChecked = new HistoricalBoolProperty(nameof(IsChecked), _isChecked, loggerFactory)
            {
                Parent = this
            }
            .DisposeItWith(Disposable);
        IsChecked
            .ViewValue
            .SubscribeAwait(async (_, _) => await Rise(new ToDoItemChangedEvent(this)))
            .DisposeItWith(Disposable);
    }

    public ReactiveCommand<ToDoItemViewModel> RemoveItem { get; set; }
    public BindableReactiveProperty<string> Content { get; set; }
    public HistoricalBoolProperty IsChecked { get; }

    public ToDoItem GetToDoItem()
    {
        return new ToDoItem(_itemId, Content.Value, IsChecked.ViewValue.Value);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return IsChecked;
    }
}