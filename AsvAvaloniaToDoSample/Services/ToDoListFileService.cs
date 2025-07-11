using System.Text.Json;
using AsvAvaloniaToDoSample.Models;

namespace AsvAvaloniaToDoSample.Services;

public static class ToDoListFileService
{
    private static readonly string _jsonFileName =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Avalonia.SimpleToDoList", "MyToDoList.txt");

    /// <summary>
    ///     Stores the given items into a file on disc
    /// </summary>
    /// <param name="itemsToSave">The items to save</param>
    public static async Task SaveToFileAsync(IEnumerable<ToDoItem> itemsToSave)
    {
        // Ensure all directories exists
        Directory.CreateDirectory(Path.GetDirectoryName(_jsonFileName)!);

        // We use a FileStream to write all items to disc
        await using var fs = File.Create(_jsonFileName);
        await JsonSerializer.SerializeAsync(fs, itemsToSave);
    }

    /// <summary>
    ///     Loads the file from disc and returns the items stored inside
    /// </summary>
    /// <returns>An IEnumerable of items loaded or null in case the file was not found</returns>
    public static async Task<IEnumerable<ToDoItem>?> LoadFromFileAsync()
    {
        try
        {
            // We try to read the saved file and return the ToDoItemsList if successful
            await using var fs = File.OpenRead(_jsonFileName);
            return await JsonSerializer.DeserializeAsync<IEnumerable<ToDoItem>>(fs);
        }
        catch (Exception e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            // In case the file was not found, we simply return null
            return null;
        }
    }
}