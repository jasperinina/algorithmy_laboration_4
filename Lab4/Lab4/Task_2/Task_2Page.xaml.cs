using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Lab4.Task_2;

public partial class Task_2Page : Page
{
    private MainWindow _mainWindow;
    private StackPanel dynamicPanel; 
    private ComboBox sortMethodComboBox;
    private TextBox inputFilePathTextBox;
    private ComboBox sortAttributeComboBox; // Основной атрибут
    private ComboBox secondaryAttributeComboBox; // Вторичный атрибут

    public Task_2Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        AddDynamicControls();
    }

    private void AddDynamicControls()
    {
        dynamicPanel = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Left
        };

        TextBlock sortMethodHeader = new TextBlock
        {
            Text = "Выберите метод сортировки",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle"),
            Margin = new Thickness(0, 0, 0, 5)
        };

        sortMethodComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 30),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };
        sortMethodComboBox.Items.Add("Прямое слияние");
        sortMethodComboBox.Items.Add("Естественное слияние");
        sortMethodComboBox.Items.Add("Многопутевое слияние");
        sortMethodComboBox.SelectedIndex = 0;

        TextBlock depthTextBlock = new TextBlock
        {
            Text = "Путь к файлу",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            Style = (Style)_mainWindow.FindResource("TextBlockStyle")
        };

        inputFilePathTextBox = new TextBox
        {
            Width = 360,
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle")
        };

        Button browseFileButton = new Button
        {
            Content = "Обзор",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            Style = (Style)_mainWindow.FindResource("OverwriteFileButtonStyle")
        };
        browseFileButton.Click += (s, e) => BrowseFileButton_Click(inputFilePathTextBox);

        TextBlock sortAttributeHeader = new TextBlock
        {
            Text = "Выберите основной атрибут сортировки",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        sortAttributeComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };

        TextBlock secondaryAttributeHeader = new TextBlock
        {
            Text = "Выберите вторичный атрибут сортировки",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        secondaryAttributeComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 30),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };

        Button executeButton = new Button
        {
            Content = "Запустить сортировку",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 50, 0, 0),
            Style = (Style)_mainWindow.FindResource("RoundedButtonStyle")
        };
        executeButton.Click += (s, e) => ExecuteSortButton_Click();

        dynamicPanel.Children.Add(sortMethodHeader);
        dynamicPanel.Children.Add(sortMethodComboBox);
        dynamicPanel.Children.Add(depthTextBlock);
        dynamicPanel.Children.Add(inputFilePathTextBox);
        dynamicPanel.Children.Add(browseFileButton);
        dynamicPanel.Children.Add(sortAttributeHeader);
        dynamicPanel.Children.Add(sortAttributeComboBox);
        dynamicPanel.Children.Add(secondaryAttributeHeader);
        dynamicPanel.Children.Add(secondaryAttributeComboBox);
        dynamicPanel.Children.Add(executeButton);

        _mainWindow.PageContentControl.Content = dynamicPanel;
    }

    private void BrowseFileButton_Click(TextBox inputFilePathTextBox)
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            inputFilePathTextBox.Text = openFileDialog.FileName;

            try
            {
                var firstLine = File.ReadLines(openFileDialog.FileName).First();
                var attributes = firstLine.Split(',');

                sortAttributeComboBox.Items.Clear();
                secondaryAttributeComboBox.Items.Clear();

                foreach (var attribute in attributes)
                {
                    sortAttributeComboBox.Items.Add(attribute.Trim());
                    secondaryAttributeComboBox.Items.Add(attribute.Trim());
                }

                if (sortAttributeComboBox.Items.Count > 0)
                {
                    sortAttributeComboBox.SelectedIndex = 0;
                }
                if (secondaryAttributeComboBox.Items.Count > 0)
                {
                    secondaryAttributeComboBox.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void ExecuteSortButton_Click()
{
    try
    {
        var sortMethod = sortMethodComboBox.SelectedItem.ToString();
        var inputFilePath = inputFilePathTextBox.Text;
        var primarySortAttribute = sortAttributeComboBox.SelectedItem.ToString();
        var secondarySortAttribute = secondaryAttributeComboBox.SelectedItem.ToString();

        // Читаем заголовок файла
        var firstLine = File.ReadLines(inputFilePath).FirstOrDefault();

        if (string.IsNullOrWhiteSpace(firstLine))
        {
            MessageBox.Show("Файл не содержит заголовков или пуст.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var headers = firstLine.Split(',');

        if (headers.Length == 0)
        {
            MessageBox.Show("Не удалось определить заголовки из файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var primaryKeyIndex = Array.IndexOf(headers, primarySortAttribute);
        var secondaryKeyIndex = Array.IndexOf(headers, secondarySortAttribute);

        if (primaryKeyIndex == -1)
        {
            MessageBox.Show($"Атрибут {primarySortAttribute} не найден в заголовке файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (secondaryKeyIndex == -1)
        {
            MessageBox.Show($"Атрибут {secondarySortAttribute} не найден в заголовке файла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        // Проверяем данные файла
        foreach (var line in File.ReadLines(inputFilePath).Skip(1))
        {
            var columns = line.Split(',');

            if (columns.Length < headers.Length)
            {
                MessageBox.Show($"Ошибка в строке данных: {line}. Ожидается не менее {headers.Length} колонок.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        var fileHandler = new FileHandler(inputFilePath, "output.txt");
        var sorter = new ExternalSorter(fileHandler);
        var stepDescription = new SortingStepDescription();

        sorter.Sort(sortMethod, primaryKeyIndex, secondaryKeyIndex,
            step =>
            {
                // Получаем значения для элемента 1
                var element1 = step.First(); // Пример, замените First() на фактическую логику
                var primaryKeyValue1 = "Значение основного ключа для элемента 1"; // Замените на реальное значение
                var secondaryKeyValue1 = "Значение вторичного ключа для элемента 1"; // Замените на реальное значение

                // Получаем значения для элемента 2
                var element2 = step.Last(); // Пример, замените Last() на фактическую логику
                var primaryKeyValue2 = "Значение основного ключа для элемента 2"; // Замените на реальное значение
                var secondaryKeyValue2 = "Значение вторичного ключа для элемента 2"; // Замените на реальное значение

                var description = stepDescription.GetStepDescription(
                    step, 
                    element1, 
                    element2, 
                    primaryKeyValue1, 
                    primaryKeyValue2, 
                    secondaryKeyValue1, 
                    secondaryKeyValue2, 
                    "Операция"
                );

                Dispatcher.Invoke(() => UpdateVisualization(step, description));
            },
            delay: 1000);


        MessageBox.Show("Сортировка завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}


    private void UpdateVisualization(List<string> step, string description)
    {
        OutputTextBox.Text += $"{description}{Environment.NewLine}";
        OutputTextBox.Text += "Текущие данные:" + Environment.NewLine;
        OutputTextBox.Text += string.Join(Environment.NewLine, step) + Environment.NewLine;
        OutputTextBox.Text += new string('-', 50) + Environment.NewLine;
    }
}
