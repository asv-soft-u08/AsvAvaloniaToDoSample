using Asv.Avalonia;

namespace AsvAvaloniaToDoSample.Pages.ToDoList.RoutedEvents;

/// <summary>
///     Represents an event triggered when todo item changes.
/// </summary>
/// <param name="source">.</param>
public sealed class ToDoItemChangedEvent(IRoutable source)
    : AsyncRoutedEvent(source, RoutingStrategy.Bubble);