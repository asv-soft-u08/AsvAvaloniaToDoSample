using Asv.Avalonia;
using AsvAvaloniaToDoSample.Models;
using AsvAvaloniaToDoSample.Pages.ToDoList;
using Material.Icons;

namespace AsvAvaloniaToDoSample.Commands;

[ExportCommand]
public class RemoveToDoCommand : ContextCommand<ToDoListViewModel, ActionArg>
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

    public override ICommandInfo Info => StaticInfo;

    public override async ValueTask<ActionArg?> InternalExecute(
        ToDoListViewModel context, ActionArg arg, CancellationToken cancel)
    {
        if (arg.Value is null || arg.SubjectId is null) return null;

        var toDoItemDict = arg.Value.AsDictionary();
        var toDoItem = new ToDoItem(
            arg.SubjectId,
            toDoItemDict["Content"].AsString(),
            toDoItemDict["IsChecked"].AsBool());

        switch (arg.Action)
        {
            case ActionArg.Kind.Add:
            {
                await context.AddItem(toDoItem, cancel);
                return new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Remove);
            }
            case ActionArg.Kind.Remove:
            {
                await context.RemoveItem(toDoItem.Id, cancel);
                return new ActionArg(toDoItem.Id, toDoItemDict, ActionArg.Kind.Add);
            }
            default: return null;
        }
    }
}