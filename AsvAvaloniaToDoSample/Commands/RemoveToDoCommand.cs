using System.Composition;
using Asv.Avalonia;
using AsvAvaloniaToDoSample.Models;
using AsvAvaloniaToDoSample.Services;
using Material.Icons;

namespace AsvAvaloniaToDoSample.Commands;

[ExportCommand]
public class RemoveToDoCommand : ContextCommand<IRoutable, ActionArg>
{
    public const string Id = $"{BaseId}.todo.remove";

    internal static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = RS.ToDoListView_RemoveItem,
        Description = RS.ToDoListCommands_RemoveDesc,
        Icon = MaterialIconKind.Remove,
        DefaultHotKey = null,
        Source = SystemModule.Instance
    };

    private readonly IToDoListService _toDoListService;

    [ImportingConstructor]
    public RemoveToDoCommand(IToDoListService toDoListService)
    {
        _toDoListService = toDoListService;
    }

    public override ICommandInfo Info => StaticInfo;

    public override async ValueTask<ActionArg?> InternalExecute(
        IRoutable context, ActionArg arg, CancellationToken cancel)
    {
        if (arg.Value is null || arg.SubjectId is null) return null;

        var toDoItemDict = arg.Value.AsDictionary();
        var toDoItem = new ToDoItem(
            arg.SubjectId,
            toDoItemDict[nameof(ToDoItem.Content)].AsString(),
            toDoItemDict[nameof(ToDoItem.IsChecked)].AsBool());

        switch (arg.Action)
        {
            case ActionArg.Kind.Add:
            {
                await _toDoListService.AddAsync(toDoItem, cancel);
                return new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Remove);
            }
            case ActionArg.Kind.Remove:
            {
                await _toDoListService.RemoveAsync(toDoItem.Id, cancel);
                return new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Add);
            }
            default: return null;
        }
    }
}