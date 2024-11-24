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
    private ComboBox valuesComboBox; // Уникальные значения основного атрибута

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
        sortAttributeComboBox.SelectionChanged += SortAttributeComboBox_SelectionChanged;

        TextBlock valuesTextBlock = new TextBlock
        {
            Text = "Выберите значение основного атрибута",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        valuesComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 30),
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
        dynamicPanel.Children.Add(valuesTextBlock);
        dynamicPanel.Children.Add(valuesComboBox);
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
                var lines = File.ReadAllLines(openFileDialog.FileName);

                if (lines.Length < 2)
                {
                    MessageBox.Show("Файл должен содержать заголовок и хотя бы одну строку данных.", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var headers = lines[0].Split(',').Select(h => h.Trim()).ToArray();
                var data = lines.Skip(1).Select(line => line.Split(',').Select(col => col.Trim()).ToArray()).ToList();

                // Выводим только те столбцы, которые имеют дубликаты (если это необходимо)
                var columnsWithDuplicates = headers.Where((header, i) =>
                    data.Select(row => row[i]).Distinct().Count() < data.Count).ToList();

                sortAttributeComboBox.Items.Clear();
                foreach (var column in columnsWithDuplicates)
                {
                    sortAttributeComboBox.Items.Add(column);
                }

                if (sortAttributeComboBox.Items.Count > 0)
                {
                    sortAttributeComboBox.SelectedIndex = 0;
                    UpdateValuesComboBox(data, headers, sortAttributeComboBox.SelectedItem.ToString());
                }

                // Фильтрация строк, которые не содержат основной атрибут будет выполняться при сортировке
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private void SortAttributeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sortAttributeComboBox.SelectedItem == null || inputFilePathTextBox.Text == string.Empty)
            return;

        var selectedAttribute = sortAttributeComboBox.SelectedItem.ToString();
        var headers = File.ReadLines(inputFilePathTextBox.Text).First().Split(',').Select(h => h.Trim()).ToArray();
        var data = File.ReadAllLines(inputFilePathTextBox.Text).Skip(1)
            .Select(line => line.Split(',').Select(col => col.Trim()).ToArray()).ToList();

        UpdateSecondaryComboBox(headers);
        UpdateValuesComboBox(data, headers, selectedAttribute);
    }

    private void UpdateValuesComboBox(List<string[]> data, string[] headers, string selectedAttribute)
    {
        int columnIndex = Array.IndexOf(headers, selectedAttribute);

        if (columnIndex == -1)
            return;

        var uniqueValues = data.Select(row => row[columnIndex]).Distinct().ToList();
        valuesComboBox.ItemsSource = uniqueValues;
    }

    private void UpdateSecondaryComboBox(string[] headers)
    {
        if (sortAttributeComboBox.SelectedItem == null)
            return;

        var primaryAttribute = sortAttributeComboBox.SelectedItem.ToString();
        secondaryAttributeComboBox.Items.Clear();

        foreach (var header in headers)
        {
            if (header != primaryAttribute)
            {
                secondaryAttributeComboBox.Items.Add(header);
            }
        }

        if (secondaryAttributeComboBox.Items.Count > 0)
        {
            secondaryAttributeComboBox.SelectedIndex = 0;
        }
    }

    private async void ExecuteSortButton_Click()
    {
        try
        {
            var sortMethod = sortMethodComboBox.SelectedItem.ToString();
            var inputFilePath = inputFilePathTextBox.Text;
            var primarySortAttribute = sortAttributeComboBox.SelectedItem.ToString();

            var headers = File.ReadLines(inputFilePath).First().Split(',');

            var primaryKeyIndex = Array.IndexOf(headers, primarySortAttribute);

            if (primaryKeyIndex == -1)
            {
                MessageBox.Show($"Атрибут {primarySortAttribute} не найден в заголовке файла.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fileHandler = new FileHandler(inputFilePath, "output.txt");
            var sorter = new ExternalSorter(fileHandler);

            // Очистите OutputTextBox перед началом сортировки
            OutputTextBox.Clear();

            // Запуск сортировки в отдельном потоке, чтобы не блокировать UI
            await Task.Run(() =>
            {
                sorter.Sort(sortMethod, primaryKeyIndex,
                    description =>
                    {
                        Dispatcher.Invoke(() => UpdateVisualization(description));
                    },
                    delay: 1000);
            });

            MessageBox.Show("Сортировка завершена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateVisualization(string description)
    {
        OutputTextBox.AppendText($"{description}{Environment.NewLine}{Environment.NewLine}");
        OutputTextBox.ScrollToEnd(); // Автоматическая прокрутка вниз
    }

}
