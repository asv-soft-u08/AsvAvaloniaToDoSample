using System.Text.Json.Serialization;

namespace AsvAvaloniaToDoSample.Models;

public class ToDoItem
{
    [JsonConstructor]
    public ToDoItem(string id, string content, bool isChecked)
    {
        Id = id;
        Content = content;
        IsChecked = isChecked;
    }

    public ToDoItem(string content, bool isChecked)
        : this(Guid.NewGuid().ToString(), content, isChecked)
    {
    }

    public string Id { get; }

    public bool IsChecked { get; set; }

    public string Content { get; set; }
}