using AsvAvaloniaToDoSample.Models;

namespace AsvAvaloniaToDoSample.Services;

public interface IToDoListFileService
{
    /// <summary>
    ///     Stores the given items into a file on disc
    /// </summary>
    public ValueTask SaveToFileAsync(IEnumerable<ToDoItem> itemsToSave, CancellationToken cancellationToken);

    /// <summary>
    ///     Loads the file from disc and returns the items stored inside
    /// </summary>
    /// <returns>An IEnumerable of items loaded</returns>
    public ValueTask<IEnumerable<ToDoItem>> LoadFromFileAsync(CancellationToken cancellationToken);
}