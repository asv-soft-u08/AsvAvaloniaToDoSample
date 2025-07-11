using System.Composition;
using Asv.Avalonia;
using AsvAvaloniaToDoSample.Pages.ToDoList;

namespace AsvAvaloniaToDoSample.Commands;

[ExportCommand]
[method: ImportingConstructor]
public class OpenToDoListCommand(INavigationService nav)
    : OpenPageCommandBase(ToDoListViewModel.PageId, nav)
{
    public override ICommandInfo Info => StaticInfo;

    #region Static

    public const string Id = $"{BaseId}.open.{ToDoListViewModel.PageId}";

    public static readonly ICommandInfo StaticInfo = new CommandInfo
    {
        Id = Id,
        Name = "Open ToDo items list",
        Description = "Command that opens ToDo items list",
        Icon = ToDoListViewModel.PageIcon,
        DefaultHotKey = null,
        Source = SystemModule.Instance
    };

    #endregion
}