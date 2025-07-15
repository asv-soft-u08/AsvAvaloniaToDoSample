using System.Composition;
using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Commands;
using AsvAvaloniaToDoSample.Models;
using AsvAvaloniaToDoSample.Pages.ToDoList.RoutedEvents;
using AsvAvaloniaToDoSample.Services;
using Avalonia.Threading;
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

    private readonly ReactiveProperty<string?> _newItemContentText;
    private readonly ObservableList<ToDoItemViewModel> _toDoItems = [];
    private readonly IToDoListFileService _toDoListFileService;

    public ToDoListViewModel()
        : this(DesignTime.CommandService, NullLoggerFactory.Instance, NullToDoListFileService.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();

        var testToDoItems = new List<ToDoItem>
        {
            new("hi", true),
            new("hhh", false),
            new("kkk ppp", true)
        };
        foreach (var testToDoItem in testToDoItems) AddItemWithoutSaving(testToDoItem);
    }

    [ImportingConstructor]
    public ToDoListViewModel(
        ICommandService cmd,
        ILoggerFactory loggerFactory,
        IToDoListFileService toDoListFileService
    )
        : base(PageId, cmd, loggerFactory)
    {
        Title = RS.ToDoListView_Title;

        _loggerFactory = loggerFactory;
        _toDoListFileService = toDoListFileService;

        _newItemContentText = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        NewItemContentHistorical = new HistoricalStringProperty(
            $"{PageId}{nameof(NewItemContentHistorical)}",
            _newItemContentText,
            loggerFactory)
        {
            Parent = this
        }.DisposeItWith(Disposable);

        ToDoItems = _toDoItems.ToNotifyCollectionChangedSlim().DisposeItWith(Disposable);

        RemoveItemReactiveCommand = new ReactiveCommand<ToDoItemViewModel>(async (itemViewModel, ct) =>
        {
            var toDoItem = itemViewModel.GetToDoItem();
            var toDoItemDict = new DictArg
            {
                ["Content"] = new StringArg(toDoItem.Content),
                ["IsChecked"] = new BoolArg(toDoItem.IsChecked)
            };
            var arg = new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Remove);
            await this.ExecuteCommand(RemoveToDoCommand.Id, arg, ct);
        }).DisposeItWith(Disposable);

        AddItemReactiveCommand = new ReactiveCommand(async (_, ct) =>
        {
            if (string.IsNullOrWhiteSpace(NewItemContentHistorical.ViewValue.Value)) return;

            var toDoItem = new ToDoItem(
                Guid.NewGuid().ToString(),
                NewItemContentHistorical.ViewValue.Value,
                false);
            var toDoItemDict = new DictArg
            {
                ["Content"] = new StringArg(toDoItem.Content),
                ["IsChecked"] = new BoolArg(toDoItem.IsChecked)
            };
            var arg = new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Add);

            // order is important here
            NewItemContentHistorical.ViewValue.Value = null;
            await this.ExecuteCommand(AddToDoCommand.Id, arg, ct);
        }).DisposeItWith(Disposable);

        InitializeData()
            .SafeFireAndForget(ex => { Logger.LogError(ex, "Error to load tasks"); });
    }

    public INotifyCollectionChangedSynchronizedViewList<ToDoItemViewModel> ToDoItems { get; }

    public HistoricalStringProperty NewItemContentHistorical { get; }

    public ReactiveCommand<ToDoItemViewModel> RemoveItemReactiveCommand { get; set; }
    public ReactiveCommand AddItemReactiveCommand { get; set; }

    public override IExportInfo Source => SystemModule.Instance;

    protected override async ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ToDoItemChangedEvent { Source: ToDoItemViewModel })
        {
            await SaveToDoTasks(CancellationToken.None);
            e.IsHandled = true;
        }

        await base.InternalCatchEvent(e);
    }

    private async Task InitializeData()
    {
        var todos = await _toDoListFileService.LoadFromFileAsync(CancellationToken.None);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var todo in todos) AddItemWithoutSaving(todo);
        });
    }

    internal void AddItemWithoutSaving(ToDoItem toDoItem)
    {
        var vm = new ToDoItemViewModel(toDoItem, _loggerFactory)
            .SetRoutableParent(this)
            .DisposeItWith(Disposable);
        ToDoItems.Add(vm);
    }

    internal async Task AddItem(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        AddItemWithoutSaving(toDoItem);
        await SaveToDoTasks(cancellationToken);
    }

    internal async Task RemoveItem(string itemId, CancellationToken cancellationToken)
    {
        var vmToDelete = ToDoItems.FirstOrDefault(i => i.GetToDoItem().Id == itemId);
        if (vmToDelete is null) return;

        ToDoItems.Remove(vmToDelete);

        // remove and dispose item, but this cause problems with historical IsChecked field 
        // Disposable.Remove(vmToDelete);

        await SaveToDoTasks(cancellationToken);
    }

    private async Task SaveToDoTasks(CancellationToken cancellationToken)
    {
        var todos = ToDoItems.Select(i => i.GetToDoItem());
        await _toDoListFileService.SaveToFileAsync(todos, cancellationToken);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return NewItemContentHistorical;

        // do we need this?
        foreach (var toDoItem in ToDoItems) yield return toDoItem;
    }

    protected override void AfterLoadExtensions()
    {
    }
}