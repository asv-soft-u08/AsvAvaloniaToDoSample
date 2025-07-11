using System.Composition;
using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Models;
using Material.Icons;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ObservableCollections;
using R3;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

[ExportPage(PageId)]
public class ToDoListViewModel : PageViewModel<ToDoListViewModel>
{
    public const string PageId = "todo-list";
    public const MaterialIconKind PageIcon = MaterialIconKind.User;

    private readonly ILoggerFactory _loggerFactory;

    public ToDoListViewModel()
        : this(DesignTime.CommandService, NullLoggerFactory.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();

        ToDoItems = new ObservableList<ToDoItemViewModel>([
            new ToDoItemViewModel(),
            new ToDoItemViewModel()
        ]).ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
    }

    [ImportingConstructor]
    public ToDoListViewModel(
        ICommandService cmd,
        ILoggerFactory loggerFactory
    )
        : base(PageId, cmd, loggerFactory)
    {
        _loggerFactory = loggerFactory;

        NewItemContent = new BindableReactiveProperty<string?>(null);
        ToDoItems = _toDoItems.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);
        RemoveItemCommand = new ReactiveCommand<ToDoItemViewModel>(RemoveItem).DisposeItWith(Disposable);
        AddItemCommand = CanAddItem.ToReactiveCommand(AddItem).DisposeItWith(Disposable);

        Title = RS.ToDoListView_Title;
    }

    private ObservableList<ToDoItemViewModel> _toDoItems { get; } = [];
    public INotifyCollectionChangedSynchronizedViewList<ToDoItemViewModel> ToDoItems { get; }

    public BindableReactiveProperty<string?> NewItemContent { get; set; }

    public ReactiveCommand<ToDoItemViewModel> RemoveItemCommand { get; set; }
    public ReactiveCommand AddItemCommand { get; set; }

    public override IExportInfo Source => SystemModule.Instance;

    private Observable<bool> CanAddItem => NewItemContent.Select(x => !string.IsNullOrWhiteSpace(x));

    private void AddItem(Unit arg)
    {
        ToDoItems.Add(new ToDoItemViewModel(new ToDoItem(NewItemContent.Value, false), _loggerFactory));
        NewItemContent.Value = null;
    }

    private void RemoveItem(ToDoItemViewModel item)
    {
        ToDoItems.Remove(item);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        return [];
    }

    protected override void AfterLoadExtensions()
    {
    }
}