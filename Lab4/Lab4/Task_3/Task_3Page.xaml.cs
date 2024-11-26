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

        public Task_3Page(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
        }

        private void btBrowseFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tBoxForFilePath.Text = openFileDialog.FileName;
            }
        }

        private string GetSelectedAlgorithm()
        {
            if (SortingAlgorithmComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                return selectedItem.Content.ToString();
            }
            return "QuickSort";
        }

        private void btForCountWordsForDoc_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tBoxForFilePath.Text))
            {
                MessageBox.Show("Выберите файл для анализа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            TextHandler textHandler = new TextHandler(tBoxForFilePath.Text);
            string algorithm = GetSelectedAlgorithm();
            string[] sortedText;

            if (algorithm == "QuickSort")
            {
                QuickSortForString quickSort = new QuickSortForString(textHandler.Text);
                sortedText = quickSort.Text;
            }
            else
            {
                RadixSort radixSort = new RadixSort(textHandler.Text);
                sortedText = radixSort.Text;
            }

            tBoxForCountWords.Text = $"Текст отсортирован с помощью {algorithm}\n" +
                                     TextHandler.CountWord(sortedText);
        }

        private void btForCountWordsForGeneratedText_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tBoxForLengthWords.Text, out int inputLengthText) || inputLengthText <= 0)
            {
                MessageBox.Show("Введите корректное число слов для генерации.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string text = TextHandler.GenerateText(inputLengthText);
            string algorithm = GetSelectedAlgorithm();
            string[] sortedText;

            if (algorithm == "QuickSort")
            {
                QuickSortForString quickSort = new QuickSortForString(text);
                sortedText = quickSort.Text;
            }
            else
            {
                RadixSort radixSort = new RadixSort(text);
                sortedText = radixSort.Text;
            }

            tBoxForCountWords.Text = $"Текст отсортирован с помощью {algorithm}\n" +
                                     TextHandler.CountWord(sortedText);
        }

        private void btForTest_Click(object sender, RoutedEventArgs e)
        {
            double[,] data = GetExecutionTimes();
            List<DataItem> items = new List<DataItem>();

            for (int i = 0; i < data.GetLength(0); i++)
            {
                items.Add(new DataItem
                {
                    WordCount = data[i, 0].ToString(),
                    QuickSort = data[i, 1].ToString(),
                    RadixSort = data[i, 2].ToString()
                });
            }

            DataListView.ItemsSource = items;
        }

        private static double[,] GetExecutionTimes()
        {
            int[] sizeText = { 100, 500, 1000, 2000, 5000, 7000, 10000, 15000, 20000 };
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

        public class DataItem
        {
            public string WordCount { get; set; }
            public string QuickSort { get; set; }
            public string RadixSort { get; set; }
        }
    }
}