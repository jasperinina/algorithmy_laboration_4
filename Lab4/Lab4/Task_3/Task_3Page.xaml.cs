using System.Windows.Controls;

namespace Lab4.Task_3;

public partial class Task_3Page : Page
{
    private MainWindow _mainWindow;
    private StackPanel dynamicPanel;
    
    public Task_3Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        InitializeComponent();
    }
}