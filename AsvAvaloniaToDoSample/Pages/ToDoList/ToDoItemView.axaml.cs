using Asv.Avalonia;
using Avalonia.Controls;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

[ExportViewFor(typeof(ToDoItemViewModel))]
public partial class ToDoItemView : UserControl
{
    public ToDoItemView()
    {
        InitializeComponent();
    }
}