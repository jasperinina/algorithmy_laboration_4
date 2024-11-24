using System.Diagnostics;
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

    private static double[,] GetExecutionTimes()
    {
        int[] sizeText = { 100, 500, 1000, 2000, 5000, 7000, 10000, 15000, 20000 };
        double[,] result = new double[sizeText.Length, 3];
        for (int i = 0; i < sizeText.Length; i++)
        {
            string text = TextHandler.GenerateText(sizeText[i]);
            Stopwatch stopwatchQ = Stopwatch.StartNew();
            QuickSortForString q = new QuickSortForString(text);
            stopwatchQ.Stop();
            Stopwatch stopwatchR = Stopwatch.StartNew();
            RadixSort r = new RadixSort(text);
            stopwatchR.Stop();

            result[i, 0] = sizeText[i];
            result[i, 1] = stopwatchQ.Elapsed.TotalMilliseconds;
            result[i, 2] = stopwatchR.Elapsed.TotalMilliseconds;
        }


        return result;
    }

    // Строка для таблицы
    public class DataItem
    {
        public string WordCount { get; set; }
        public string QuickSort { get; set; }
        public string RadixSort { get; set; }
    }

    // Замеры времени для таблицы
    private void btForTest_Click(object sender, System.Windows.RoutedEventArgs e)
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

    // Подсчет слов для готового текста
    private void btForCountWordsForDoc_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        string path = tBoxForLengthWords.Text;
        TextHandler TH = new TextHandler(path);
        string text = TH.Text;
        tBoxForCountWords.Text = TextHandler.CountWord(text.Split(" "));
    }

    // Кнопка для подсчета слов для сгенерированного текста
    private void btForCountWordsForGeneratedText_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        int inputLengthText = Convert.ToInt32(tBoxForLengthWords.Text);
        string text = TextHandler.GenerateText(inputLengthText);
        tBoxForCountWords.Text = TextHandler.CountWord(text.Split(" "));
    }
}