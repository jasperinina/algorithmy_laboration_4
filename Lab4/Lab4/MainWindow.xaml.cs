using System.Windows;
using Lab4.Task_1;
using Lab4.Task_2;
using Lab4.Task_3;

namespace Lab4;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }
    
    // Переход на страницу со стеком
    private void Task_1RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        // Очистить динамически добавленные элементы на текущей странице
        /*
        if (MainFrame.Content is StackPage stackPage)
        {
            stackPage.ClearDynamicElements();
        }*/
        
        MainFrame.Navigate(new Task_1Page(this));
    }

    // Переход на страницу с очередью
    private void Task_2RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        // Очистить динамически добавленные элементы на текущей странице
        /*
        if (MainFrame.Content is StackPage stackPage)
        {
            stackPage.ClearDynamicElements();
        }*/
        
        MainFrame.Navigate(new Task_2Page(this));
    }
    
    // Переход на страницу с алгоритмами
    private void Task_3RadioButton_Checked(object sender, RoutedEventArgs e)
    {
        // Очистить динамически добавленные элементы на текущей странице
        /*
        if (MainFrame.Content is StackPage stackPage)
        {
            stackPage.ClearDynamicElements();
        }*/
            
        MainFrame.Navigate(new Task_3Page(this));
    }
}