using AsvAvaloniaToDoSample.Models;
using ObservableCollections;

namespace AsvAvaloniaToDoSample.Services;

public interface IToDoListService
{
    public ObservableList<ToDoItem> Items { get; }

    /// <summary>
    ///     Refreshes the todos and populate the observable collection
    /// </summary>
    /// <returns>An IEnumerable of items loaded</returns>
    public ValueTask RefreshAsync(CancellationToken cancellationToken);

    public ValueTask AddAsync(ToDoItem toDoItem, CancellationToken cancellationToken);

    public ValueTask UpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken);

    public ValueTask RemoveAsync(string toDoItemId, CancellationToken cancellationToken);
}