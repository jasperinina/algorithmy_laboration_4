using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Lab4.Task_3
{
    public partial class Task_3Page : Page
    {
        private MainWindow _mainWindow;
        private ComboBox SortingAlgorithmComboBox;
        private ComboBox DataAlgorithmComboBox;
        private TextBox inputFilePathTextBox;
        private TextBox tBoxForLengthWordsTextBox;
        private Button browseFileButton;
        private StackPanel dynamicPanel;

        // Добавляем поля для заголовков
        private TextBlock wordCountHeader;
        private TextBlock filePathHeader;

        public Task_3Page(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            AddDynamicControls();
        }

        private void AddDynamicControls()
        {
            dynamicPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Left,
            };

            // Заголовок для сортировки
            TextBlock sortHeader = new TextBlock
            {
                Text = "Выберите метод сортировки",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0),
                Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle")
            };

            // Выбор алгоритма сортировки
            SortingAlgorithmComboBox = new ComboBox
            {
                Width = 360,
                Margin = new Thickness(0, 8, 0, 30),
                Style = (Style)_mainWindow.FindResource("PopUp")
            };
            SortingAlgorithmComboBox.Items.Add("Быстрая сортировка");
            SortingAlgorithmComboBox.Items.Add("Сортировка по основанию");
            SortingAlgorithmComboBox.SelectedIndex = 0;

            // Заголовок для данных
            TextBlock Header = new TextBlock
            {
                Text = "Введите данные для тестирования",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 20),
                Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle")
            };

            // Заголовок для выбора способа ввода данных
            TextBlock dataInputHeader = new TextBlock
            {
                Text = "Способ ввода данных",
                Margin = new Thickness(0, 0, 0, 8),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("TextBlockStyle")
            };

            // Выбор способа ввода данных
            DataAlgorithmComboBox = new ComboBox
            {
                Width = 360,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 20),
                Style = (Style)_mainWindow.FindResource("PopUp")
            };
            DataAlgorithmComboBox.Items.Add("Использовать файл");
            DataAlgorithmComboBox.Items.Add("Сгенерировать текст");
            DataAlgorithmComboBox.SelectedIndex = 0;
            DataAlgorithmComboBox.SelectionChanged += DataAlgorithmComboBox_SelectionChanged;

            // Поле для выбора файла
            filePathHeader = new TextBlock
            {
                Text = "Путь к файлу",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 8),
                Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
                Visibility = Visibility.Visible
            };

            inputFilePathTextBox = new TextBox
            {
                Width = 360,
                Margin = new Thickness(0, 0, 0, 0),
                Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle"),
                Visibility = Visibility.Visible
            };

            browseFileButton = new Button
            {
                Content = "Обзор",
                Width = 360,
                Margin = new Thickness(0, 20, 0, 165),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("OverwriteFileButtonStyle"),
                Visibility = Visibility.Visible
            };
            browseFileButton.Click += BrowseFileButton_Click;

            // Инициализируем заголовок количества слов как поле класса
            wordCountHeader = new TextBlock
            {
                Text = "Количество слов для генерации",
                Margin = new Thickness(0, 0, 0, 8),
                Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
                HorizontalAlignment = HorizontalAlignment.Left,
                Visibility = Visibility.Collapsed
            };

            // Поле для ввода количества слов
            tBoxForLengthWordsTextBox = new TextBox
            {
                Width = 360,
                Margin = new Thickness(0, 0, 0, 225),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle"),
                Visibility = Visibility.Collapsed
            };

            // Кнопка для запуска сортировки
            Button executeButton = new Button
            {
                Content = "Запустить сортировку",
                Width = 360,
                Margin = new Thickness(0, 20, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("RoundedButtonStyle")
            };
            executeButton.Click += ExecuteButton_Click;

            // Кнопка для замеров времени
            Button measureTimeButton = new Button
            {
                Content = "Сравнение алгоритмов",
                Width = 360,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("RoundedButtonGraphStyle")
            };
            measureTimeButton.Click += MeasureTimeButton_Click;

            // Добавление элементов на панель
            dynamicPanel.Children.Add(sortHeader);
            dynamicPanel.Children.Add(SortingAlgorithmComboBox);
            dynamicPanel.Children.Add(Header);
            dynamicPanel.Children.Add(dataInputHeader);
            dynamicPanel.Children.Add(DataAlgorithmComboBox);
            dynamicPanel.Children.Add(filePathHeader); // Добавляем заголовок пути к файлу
            dynamicPanel.Children.Add(inputFilePathTextBox);
            dynamicPanel.Children.Add(browseFileButton);
            dynamicPanel.Children.Add(wordCountHeader); // Добавляем заголовок количества слов
            dynamicPanel.Children.Add(tBoxForLengthWordsTextBox);
            dynamicPanel.Children.Add(executeButton);
            dynamicPanel.Children.Add(measureTimeButton);

            // Установка панели в главное окно
            _mainWindow.PageContentControl.Content = dynamicPanel;
        }

        private void DataAlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Проверяем, выбрана ли опция "Использовать файл"
            bool useFile = DataAlgorithmComboBox.SelectedIndex == 0;

            // Отображаем/скрываем элементы для ввода пути к файлу
            filePathHeader.Visibility = useFile ? Visibility.Visible : Visibility.Collapsed;
            inputFilePathTextBox.Visibility = useFile ? Visibility.Visible : Visibility.Collapsed;
            browseFileButton.Visibility = useFile ? Visibility.Visible : Visibility.Collapsed;

            // Отображаем/скрываем элементы для ввода количества слов
            wordCountHeader.Visibility = useFile ? Visibility.Collapsed : Visibility.Visible;
            tBoxForLengthWordsTextBox.Visibility = useFile ? Visibility.Collapsed : Visibility.Visible;
        }
        
        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                inputFilePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedAlgorithm = SortingAlgorithmComboBox.SelectedItem.ToString();
            string[] sortedText;

            if (DataAlgorithmComboBox.SelectedIndex == 0) // Использовать файл
            {
                if (string.IsNullOrWhiteSpace(inputFilePathTextBox.Text))
                {
                    MessageBox.Show("Укажите путь к файлу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    TextHandler textHandler = new TextHandler(inputFilePathTextBox.Text);
                    sortedText = SortText(textHandler.Text, selectedAlgorithm);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка чтения файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else // Сгенерировать текст
            {
                if (!int.TryParse(tBoxForLengthWordsTextBox.Text, out int wordCount) || wordCount <= 0)
                {
                    MessageBox.Show("Введите корректное число слов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string text = TextHandler.GenerateText(wordCount);
                sortedText = SortText(text, selectedAlgorithm);
            }

            tBoxForSortedText.Text = string.Join(" ", sortedText); // Добавляем отсортированный текст
            tBoxForCountWords.Text = $"Текст отсортирован с помощью {selectedAlgorithm}\n" + TextHandler.CountWord(sortedText);
        }
        
        private void MeasureTimeButton_Click(object sender, RoutedEventArgs e)
        {
            double[,] data = GetExecutionTimes();
            List<DataItem> items = new List<DataItem>();

            for (int i = 0; i < data.GetLength(0); i++)
            {
                items.Add(new DataItem
                {
                    WordCount = data[i, 0].ToString(),
                    QuickSort = $"{data[i, 1]:F2} ms",
                    RadixSort = $"{data[i, 2]:F2} ms"
                });
            }

            // Отображение данных в ListView
            DataListView.ItemsSource = items;
        }
        
        private string[] SortText(string text, string algorithm)
        {
            if (algorithm == "Быстрая сортировка")
            {
                QuickSortForString quickSort = new QuickSortForString(text);
                return quickSort.Text;
            }
            else if (algorithm == "Сортировка по основанию")
            {
                RadixSort radixSort = new RadixSort(text);
                return radixSort.Text;
            }
            else
            {
                throw new ArgumentException("Неизвестный алгоритм сортировки.");
            }
        }

        private static double[,] GetExecutionTimes()
        {
            int[] sizeText = { 100, 500, 1000, 2000, 5000, 10000 };
            double[,] result = new double[sizeText.Length, 3];

            for (int i = 0; i < sizeText.Length; i++)
            {
                string text = TextHandler.GenerateText(sizeText[i]);
                Stopwatch stopwatchQ = Stopwatch.StartNew();
                QuickSortForString quickSort = new QuickSortForString(text);
                stopwatchQ.Stop();
                Stopwatch stopwatchR = Stopwatch.StartNew();
                RadixSort radixSort = new RadixSort(text);
                stopwatchR.Stop();

                result[i, 0] = sizeText[i];
                result[i, 1] = stopwatchQ.Elapsed.TotalMilliseconds;
                result[i, 2] = stopwatchR.Elapsed.TotalMilliseconds;
            }

            return result;
        }
    }

    public class DataItem
    {
        public string WordCount { get; set; }
        public string QuickSort { get; set; }
        public string RadixSort { get; set; }
    }
}