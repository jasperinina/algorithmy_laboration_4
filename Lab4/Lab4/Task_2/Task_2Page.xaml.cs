using System.Windows.Controls;

namespace Lab4.Task_2;

public partial class Task_2Page : Page
{
    private MainWindow _mainWindow;
    private StackPanel dynamicPanel;
    
    public Task_2Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        InitializeComponent();
    }
}