using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using R3;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

public class ToDoItemViewModel : RoutableViewModel
{
    public const string PageId = "todo-item";

    private readonly string _itemId;

    public ToDoItemViewModel()
        : this(new ToDoItem("Hello", false), NullLoggerFactory.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();
    }

    public ToDoItemViewModel(ToDoItem toDoItem, ILoggerFactory loggerFactory)
        : base(new NavigationId(PageId, toDoItem.Id), loggerFactory)
    {
        _itemId = toDoItem.Id;

        Content = new BindableReactiveProperty<string>(toDoItem.Content).DisposeItWith(Disposable);
        IsChecked = new BindableReactiveProperty<bool>(toDoItem.IsChecked).DisposeItWith(Disposable);
    }

    public BindableReactiveProperty<string> Content { get; set; }
    public BindableReactiveProperty<bool> IsChecked { get; set; }

    public ToDoItem GetToDoItem()
    {
        return new ToDoItem(_itemId, Content.Value, IsChecked.Value);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }
}