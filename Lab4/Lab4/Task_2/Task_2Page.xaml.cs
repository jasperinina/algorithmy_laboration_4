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
    private ComboBox filterAttributeComboBox; // Атрибут фильтрации
    private ComboBox secondaryAttributeComboBox; // Вторичный атрибут сортировки
    private ComboBox filterValueComboBox; // Значения для фильтрации

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

        // Заголовок выбора метода сортировки
        TextBlock sortMethodHeader = new TextBlock
        {
            Text = "Выберите метод сортировки",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle"),
            Margin = new Thickness(0, 0, 0, 0)
        };

        // Комбобокс для выбора метода сортировки
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

        // Заголовок выбора метода сортировки
        TextBlock Header = new TextBlock
        {
            Text = "Введите данные для тестирования",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle"),
            Margin = new Thickness(0, 0, 0, 0)
        };

        // Заголовок выбора файла
        TextBlock filePathTextBlock = new TextBlock
        {
            Text = "Путь к файлу",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            Style = (Style)_mainWindow.FindResource("TextBlockStyle")
        };

        // Текстовое поле для ввода пути к файлу
        inputFilePathTextBox = new TextBox
        {
            Width = 360,
            Margin = new Thickness(0, 0, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle")
        };

        // Кнопка для открытия диалога выбора файла
        Button browseFileButton = new Button
        {
            Content = "Обзор",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 20, 0, 8),
            Style = (Style)_mainWindow.FindResource("OverwriteFileButtonStyle")
        };
        browseFileButton.Click += (s, e) => BrowseFileButton_Click(inputFilePathTextBox);

        // Заголовок выбора атрибута фильтрации
        TextBlock filterAttributeHeader = new TextBlock
        {
            Text = "Выберите атрибут фильтрации",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        // Комбобокс для выбора атрибута фильтрации
        filterAttributeComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };
        filterAttributeComboBox.SelectionChanged += FilterAttributeComboBox_SelectionChanged;

        // Заголовок выбора значения фильтрации
        TextBlock filterValueTextBlock = new TextBlock
        {
            Text = "Выберите значение фильтрации",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        // Комбобокс для выбора значения фильтрации
        filterValueComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };

        // Заголовок выбора вторичного атрибута сортировки
        TextBlock secondaryAttributeHeader = new TextBlock
        {
            Text = "Выберите ключ сортировки",
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
            Margin = new Thickness(0, 20, 0, 0)
        };

        // Комбобокс для выбора вторичного атрибута сортировки
        secondaryAttributeComboBox = new ComboBox
        {
            Width = 360,
            Margin = new Thickness(0, 8, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            Style = (Style)_mainWindow.FindResource("PopUp")
        };

        // Кнопка для запуска сортировки
        Button executeButton = new Button
        {
            Content = "Запустить сортировку",
            Width = 360,
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(0, 67, 0, 0),
            Style = (Style)_mainWindow.FindResource("RoundedButtonStyle")
        };
        executeButton.Click += (s, e) => ExecuteSortButton_Click();

        dynamicPanel.Children.Add(sortMethodHeader);
        dynamicPanel.Children.Add(sortMethodComboBox);
        dynamicPanel.Children.Add(Header);
        dynamicPanel.Children.Add(filePathTextBlock);
        dynamicPanel.Children.Add(inputFilePathTextBox);
        dynamicPanel.Children.Add(browseFileButton);
        dynamicPanel.Children.Add(filterAttributeHeader);
        dynamicPanel.Children.Add(filterAttributeComboBox);
        dynamicPanel.Children.Add(filterValueTextBlock);
        dynamicPanel.Children.Add(filterValueComboBox);
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

                // Заполнение комбобокса атрибутов фильтрации всеми заголовками
                filterAttributeComboBox.Items.Clear();
                foreach (var column in headers)
                {
                    filterAttributeComboBox.Items.Add(column);
                }

                if (filterAttributeComboBox.Items.Count > 0)
                {
                    filterAttributeComboBox.SelectedIndex = 0;
                    UpdateFilterValueComboBox(data, headers, filterAttributeComboBox.SelectedItem.ToString());
                }

                // Заполнение комбобокса вторичных атрибутов
                UpdateSecondaryComboBox(headers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении файла: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private void FilterAttributeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (filterAttributeComboBox.SelectedItem == null || string.IsNullOrEmpty(inputFilePathTextBox.Text))
            return;

        var selectedAttribute = filterAttributeComboBox.SelectedItem.ToString();
        var headers = File.ReadLines(inputFilePathTextBox.Text).First().Split(',').Select(h => h.Trim()).ToArray();
        var data = File.ReadAllLines(inputFilePathTextBox.Text).Skip(1)
            .Select(line => line.Split(',').Select(col => col.Trim()).ToArray()).ToList();

        UpdateFilterValueComboBox(data, headers, selectedAttribute);
    }

    private void UpdateFilterValueComboBox(List<string[]> data, string[] headers, string selectedAttribute)
    {
        int columnIndex = Array.IndexOf(headers, selectedAttribute);

        if (columnIndex == -1)
            return;

        var uniqueValues = data.Select(row => row[columnIndex]).Distinct().ToList();
        filterValueComboBox.ItemsSource = uniqueValues;
        if (uniqueValues.Count > 0)
            filterValueComboBox.SelectedIndex = 0;
    }

    private void UpdateSecondaryComboBox(string[] headers)
    {
        if (filterAttributeComboBox.SelectedItem == null)
            return;

        var primaryAttribute = filterAttributeComboBox.SelectedItem.ToString();
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
            var sortMethod = sortMethodComboBox.SelectedItem?.ToString();
            var inputFilePath = inputFilePathTextBox.Text;
            var filterAttribute = filterAttributeComboBox.SelectedItem?.ToString();
            var filterValue = filterValueComboBox.SelectedItem?.ToString(); // Значение для фильтрации
            var secondarySortAttribute = secondaryAttributeComboBox.SelectedItem?.ToString(); // Вторичный атрибут

            // Валидация вводимых данных
            if (string.IsNullOrWhiteSpace(sortMethod))
            {
                MessageBox.Show("Выберите метод сортировки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(inputFilePath) || !File.Exists(inputFilePath))
            {
                MessageBox.Show("Укажите путь к файлу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(filterAttribute))
            {
                MessageBox.Show("Выберите атрибут фильтрации.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(filterValue))
            {
                MessageBox.Show("Выберите значение фильтрации.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(secondarySortAttribute))
            {
                MessageBox.Show("Выберите вторичный атрибут сортировки.", "Ошибка", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            var headers = File.ReadLines(inputFilePath).First().Split(',').Select(h => h.Trim()).ToArray();
            var filterKeyIndex = Array.IndexOf(headers, filterAttribute);
            var secondaryKeyIndex = Array.IndexOf(headers, secondarySortAttribute);

            if (filterKeyIndex == -1)
            {
                MessageBox.Show($"Атрибут фильтрации '{filterAttribute}' не найден в заголовке файла.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (secondaryKeyIndex == -1)
            {
                MessageBox.Show($"Вторичный атрибут сортировки '{secondarySortAttribute}' не найден в заголовке файла.",
                    "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var fileHandler = new FileHandler(inputFilePath, "output.txt");
            var sorter = new ExternalSorter(fileHandler);

            // Очистка области вывода перед началом сортировки
            OutputTextBox.Clear();

            // Создание объекта Progress для обновления UI
            var progress = new Progress<string>(description => UpdateVisualization(description));

            // Запуск сортировки в отдельном потоке, чтобы не блокировать UI
            await Task.Run(() =>
            {
                sorter.Sort(sortMethod, filterKeyIndex, filterValue, secondaryKeyIndex,
                    progress);
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