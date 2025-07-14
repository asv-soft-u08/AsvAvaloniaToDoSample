using AsvAvaloniaToDoSample.Models;

namespace AsvAvaloniaToDoSample.Services;

public class NullToDoListFileService : IToDoListFileService
{
    public static IToDoListFileService Instance { get; } = new NullToDoListFileService();

    public ValueTask SaveToFileAsync(IEnumerable<ToDoItem> itemsToSave, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<IEnumerable<ToDoItem>> LoadFromFileAsync(CancellationToken cancellationToken)
    {
        return ValueTask.FromResult(Enumerable.Empty<ToDoItem>());
    }
}