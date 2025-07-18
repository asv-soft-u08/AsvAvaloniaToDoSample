using Asv.Avalonia;
using AsvAvaloniaToDoSample.Models;

namespace AsvAvaloniaToDoSample.Pages.ToDoList.RoutedEvents;

/// <summary>
///     Represents an event triggered when todo item changes.
/// </summary>
/// <param name="source">.</param>
public sealed class ToDoItemChangedEvent(IRoutable source, ToDoItem newToDoItem)
    : AsyncRoutedEvent(source, RoutingStrategy.Bubble)
{
    public readonly ToDoItem NewToDoItem = newToDoItem;
}