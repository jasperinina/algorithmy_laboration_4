using System.Windows.Controls;

namespace Lab4.Task_1;

public partial class Task_1Page : Page
{
    private MainWindow _mainWindow;
    private StackPanel dynamicPanel;
    
    public Task_1Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        InitializeComponent();
    }
}