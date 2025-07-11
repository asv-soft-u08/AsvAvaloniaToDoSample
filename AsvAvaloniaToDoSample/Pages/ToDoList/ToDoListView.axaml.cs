using Asv.Avalonia;
using Avalonia.Controls;

namespace AsvAvaloniaToDoSample.Pages.ToDoList;

[ExportViewFor(typeof(ToDoListViewModel))]
public partial class ToDoListView : UserControl
{
    public ToDoListView()
    {
        InitializeComponent();
    }
}