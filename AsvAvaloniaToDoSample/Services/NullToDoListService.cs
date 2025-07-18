using AsvAvaloniaToDoSample.Models;
using ObservableCollections;

namespace AsvAvaloniaToDoSample.Services;

public class NullToDoListService : IToDoListService
{
    public static IToDoListService Instance { get; } = new NullToDoListService();

    public ObservableList<ToDoItem> Items { get; } = [];

    public ValueTask AddAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RemoveAsync(string toDoItemId, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask RefreshAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }
}