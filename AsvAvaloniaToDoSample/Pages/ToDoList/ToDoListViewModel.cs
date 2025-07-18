using System.Composition;
using Asv.Avalonia;
using Asv.Common;
using AsvAvaloniaToDoSample.Commands;
using AsvAvaloniaToDoSample.Models;
using AsvAvaloniaToDoSample.Pages.ToDoList.RoutedEvents;
using AsvAvaloniaToDoSample.Services;
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
    private readonly ISynchronizedView<ToDoItem, ToDoItemViewModel> _toDoItemsView;
    private readonly IToDoListService _toDoListService;

    public ToDoListViewModel()
        : this(DesignTime.CommandService, NullLoggerFactory.Instance, NullToDoListService.Instance)
    {
        DesignTime.ThrowIfNotDesignMode();

        var testToDoItems = new List<ToDoItem>
        {
            new("hi", true),
            new("hhh", false),
            new("kkk ppp", true)
        };
        foreach (var testToDoItem in testToDoItems)
        {
            var vm = new ToDoItemViewModel(testToDoItem, RemoveItem, _loggerFactory)
                .SetRoutableParent(this)
                .DisposeItWith(Disposable);
            ToDoItems.Add(vm);
        }
    }

    [ImportingConstructor]
    public ToDoListViewModel(
        ICommandService cmd,
        ILoggerFactory loggerFactory,
        IToDoListService toDoListService
    )
        : base(PageId, cmd, loggerFactory)
    {
        Title = RS.ToDoListView_Title;

        _loggerFactory = loggerFactory;
        _toDoListService = toDoListService;

        _newItemContentText = new ReactiveProperty<string?>().DisposeItWith(Disposable);
        NewItemContentHistorical = new HistoricalStringProperty(
            nameof(NewItemContentHistorical),
            _newItemContentText,
            loggerFactory)
        {
            Parent = this
        }.DisposeItWith(Disposable);

        RemoveItem = new ReactiveCommand<ToDoItemViewModel>(async (itemViewModel, ct) =>
        {
            var toDoItem = itemViewModel.GetToDoItem();
            var toDoItemDict = new DictArg
            {
                [nameof(ToDoItem.Content)] = new StringArg(toDoItem.Content),
                [nameof(ToDoItem.IsChecked)] = new BoolArg(toDoItem.IsChecked)
            };
            var arg = new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Remove);
            await this.ExecuteCommand(RemoveToDoCommand.Id, arg, ct);
        }).DisposeItWith(Disposable);

        AddItem = new ReactiveCommand(async (_, ct) =>
        {
            if (string.IsNullOrWhiteSpace(NewItemContentHistorical.ViewValue.Value)) return;

            var toDoItem = new ToDoItem(
                NewItemContentHistorical.ViewValue.Value,
                false);
            var toDoItemDict = new DictArg
            {
                [nameof(ToDoItem.Content)] = new StringArg(toDoItem.Content),
                [nameof(ToDoItem.IsChecked)] = new BoolArg(toDoItem.IsChecked)
            };
            var arg = new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Add);

            // order is important here
            NewItemContentHistorical.ViewValue.Value = null;
            await this.ExecuteCommand(AddToDoCommand.Id, arg, ct);
        }).DisposeItWith(Disposable);
        
        _toDoItemsView = _toDoListService.Items.CreateView(item =>
            {
                return new ToDoItemViewModel(item, RemoveItem, _loggerFactory)
                    .SetRoutableParent(this)
                    .DisposeItWith(Disposable);
            })
            .DisposeItWith(Disposable);
        // _toDoItemsView.SetRoutableParent(this).DisposeItWith(Disposable);
        // _toDoItemsView.DisposeMany(Disposable);
        ToDoItems = _toDoItemsView.ToNotifyCollectionChanged().DisposeItWith(Disposable);

        _toDoListService.RefreshAsync(CancellationToken.None)
            .SafeFireAndForget(ex => { Logger.LogError(ex, "Error to load tasks"); });
    }

    public INotifyCollectionChangedSynchronizedViewList<ToDoItemViewModel> ToDoItems { get; }

    public HistoricalStringProperty NewItemContentHistorical { get; }

    public ReactiveCommand<ToDoItemViewModel> RemoveItem { get; set; }
    public ReactiveCommand AddItem { get; set; }

    public override IExportInfo Source => SystemModule.Instance;

    protected override async ValueTask InternalCatchEvent(AsyncRoutedEvent e)
    {
        if (e is ToDoItemChangedEvent { Source: ToDoItemViewModel } toDoItemChanged)
        {
            await _toDoListService.UpdateAsync(toDoItemChanged.NewToDoItem, CancellationToken.None);
            e.IsHandled = true;
        }

        await base.InternalCatchEvent(e);
    }

    public override IEnumerable<IRoutable> GetRoutableChildren()
    {
        yield return NewItemContentHistorical;

        foreach (var toDoItem in ToDoItems) yield return toDoItem;
    }

    protected override void AfterLoadExtensions()
    {
    }
}