using System.Composition;
using System.Text.Json;
using Asv.Avalonia;
using AsvAvaloniaToDoSample.Models;

namespace AsvAvaloniaToDoSample.Services;

[Export(typeof(IToDoListFileService))]
[Shared]
public class ToDoListFileService : IToDoListFileService
{
    private readonly string _dataFilePath;

    [ImportingConstructor]
    public ToDoListFileService(IAppPath appPath)
    {
        _dataFilePath = Path.Join(appPath.UserDataFolder, "MyToDoList.json");
        Console.WriteLine(_dataFilePath);
    }

    public async ValueTask SaveToFileAsync(IEnumerable<ToDoItem> itemsToSave, CancellationToken cancellationToken)
    {
        await using var fs = File.Create(_dataFilePath);
        await JsonSerializer.SerializeAsync(fs, itemsToSave, cancellationToken: cancellationToken);
    }

    public async ValueTask<IEnumerable<ToDoItem>> LoadFromFileAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var fs = File.OpenRead(_dataFilePath);
            return await JsonSerializer.DeserializeAsync<IEnumerable<ToDoItem>>(fs,
                cancellationToken: cancellationToken) ?? [];
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            return [];
        }
    }
}