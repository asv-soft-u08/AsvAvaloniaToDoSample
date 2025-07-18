using System.Composition;
using System.Text.Json;
using Asv.Avalonia;
using AsvAvaloniaToDoSample.Models;
using ObservableCollections;

namespace AsvAvaloniaToDoSample.Services;

[Export(typeof(IToDoListService))]
[Shared]
public class FileSystemToDoListService : IToDoListService
{
    private readonly string _dataFilePath;

    [ImportingConstructor]
    public FileSystemToDoListService(IAppPath appPath)
    {
        _dataFilePath = Path.Join(appPath.UserDataFolder, "MyToDoList.json");
    }

    public ObservableList<ToDoItem> Items { get; } = [];

    public async ValueTask RefreshAsync(CancellationToken cancellationToken)
    {
        await using var fs = File.OpenRead(_dataFilePath);
        var refreshedToDos = await JsonSerializer
            .DeserializeAsync<IEnumerable<ToDoItem>>(fs, cancellationToken: cancellationToken) ?? [];

        Items.Clear();
        Items.AddRange(refreshedToDos);
    }

    public async ValueTask AddAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        Items.Add(toDoItem);
        await SaveToFileAsync(cancellationToken);
    }

    public async ValueTask UpdateAsync(ToDoItem toDoItem, CancellationToken cancellationToken)
    {
        var itemToUpdate = Items.FirstOrDefault(i => i.Id == toDoItem.Id);
        if (itemToUpdate is null) return;

        itemToUpdate.IsChecked = toDoItem.IsChecked;
        itemToUpdate.Content = toDoItem.Content;

        await SaveToFileAsync(cancellationToken);
    }

    public async ValueTask RemoveAsync(string toDoItemId, CancellationToken cancellationToken)
    {
        var itemToRemove = Items.FirstOrDefault(i => i.Id == toDoItemId);
        if (itemToRemove is null) return;

        Items.Remove(itemToRemove);

        await SaveToFileAsync(cancellationToken);
    }

    private async ValueTask SaveToFileAsync(CancellationToken cancellationToken)
    {
        await using var fs = File.Create(_dataFilePath);
        await JsonSerializer.SerializeAsync(fs, Items, cancellationToken: cancellationToken);
    }
}