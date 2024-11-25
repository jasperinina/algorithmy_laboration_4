using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using Lab4.Task_1.Sorting_Algorithms;
using System.Windows.Input;
using System.Threading;

namespace Lab4.Task_1;

public partial class Task_1Page : Page
{
    private MainWindow _mainWindow;
    private List<int> numbers;
    private List<(int, int, bool)> sortSteps;
    private List<Rectangle> rectangles;
    private double RectangleWidth;
    private int maxNumber;
    private int currentStepIndex;
    private bool isAnimating;
    private SortingAlgorithm selectedSortingAlgorithm;
    private DataGenerator dataGenerator;
    private double Timeset;

    public Task_1Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        InitializeComponent();
        InitializeSortingAlgorithms();
        dataGenerator = new DataGenerator();
        Timeset = 1;
    }
    private void InitializeSortingAlgorithms()
    {
        selectedSortingAlgorithm = new BubbleSort();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeData();
        DrawRectangles();
    }
    private void InitializeData()
    {
        numbers = dataGenerator.ArrayGenerate(50, 50).ToList();
        sortSteps = new List<(int, int, bool)>();
        rectangles = new List<Rectangle>();
        selectedSortingAlgorithm.Sort(numbers.ToArray(), sortSteps);
        maxNumber = numbers.Max();
        RectangleWidth = Canvas.ActualWidth / numbers.Count;
    }

    private void DrawRectangles()
    {
        double Spacing = 0;
        foreach (int number in numbers)
        {
            double height = (number) * Canvas.ActualHeight * 0.95 / maxNumber;

            Rectangle rect = new Rectangle
            {
                Width = RectangleWidth,
                Height = height,
                Fill = Brushes.Blue,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };

            Canvas.SetLeft(rect, Spacing);
            Canvas.SetBottom(rect, 0);
            Canvas.Children.Add(rect);
            rectangles.Add(rect);

            Spacing += RectangleWidth;
        }
    }
    private void TimeSetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (TimeSetSlider != null)
        {
            Timeset = TimeSetSlider.Value;
        }
    }
    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isAnimating)
        {
            isAnimating = true;
            await ContinueAnimation();
        }
    }
    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        isAnimating = false;
    }
    private async void NextButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isAnimating && currentStepIndex < sortSteps.Count)
        {
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item1], rectangles[sortSteps[currentStepIndex].Item2], sortSteps[currentStepIndex].Item3);
            currentStepIndex++;
        }
    }
    private async void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isAnimating && currentStepIndex > 0)
        {
            currentStepIndex--;
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item2], rectangles[sortSteps[currentStepIndex].Item1], sortSteps[currentStepIndex].Item3);
        }
    }
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        isAnimating = false;
        currentStepIndex = 0;
        Canvas.Children.Clear();
        InitializeData();
        DrawRectangles();
        LogTextBox.Text = string.Empty;
    }
    private void SortingAlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBoxItem selectedItem = (ComboBoxItem)SortingAlgorithmComboBox.SelectedItem;
        string selectedAlgorithm = selectedItem.Tag.ToString();

        switch (selectedAlgorithm)
        {
            case "BubbleSort":
                selectedSortingAlgorithm = new BubbleSort();
                break;
            case "SelectionSort":
                selectedSortingAlgorithm = new SelectionSort();
                break;
            case "QuickSort":
                selectedSortingAlgorithm = new QuickSort();
                break;
            case "HeapSort":
                selectedSortingAlgorithm = new HeapSort();
                break;
            default:
                selectedSortingAlgorithm = new BubbleSort();
                break;
        }

        // Сбросить состояние и перерисовать фигуры с новым алгоритмом
        ResetButton_Click(sender, e);
    }
    private async Task ContinueAnimation()
    {
        while (isAnimating && currentStepIndex < sortSteps.Count)
        {
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item1], rectangles[sortSteps[currentStepIndex].Item2], sortSteps[currentStepIndex].Item3);
            currentStepIndex++;
        }
        isAnimating = false;
    }
    private async Task SwapElements(Rectangle shape1, Rectangle shape2, bool shouldSwap)
    {
        double height1 = Math.Round(shape1.Height * maxNumber / Canvas.ActualHeight * 0.95, 0);
        double height2 = Math.Round(shape2.Height * maxNumber / Canvas.ActualHeight * 0.95, 0);
        if (shouldSwap)
        {
            // Меняем цвет на зеленый перед анимацией
            shape1.Fill = Brushes.Green;
            shape2.Fill = Brushes.Green;

            // Получаем текущие позиции элементов
            double shape1X = Canvas.GetLeft(shape1);
            double shape2X = Canvas.GetLeft(shape2);

            // Анимируем перемещение shape1 на место shape2
            await Animate(shape1, shape2, shape2X, shape1X);

            // Возвращаем цвет обратно на синий после анимации
            shape1.Fill = Brushes.Blue;
            shape2.Fill = Brushes.Blue;

            // Обновляем список прямоугольников
            int index1 = rectangles.IndexOf(shape1);
            int index2 = rectangles.IndexOf(shape2);
            (rectangles[index1], rectangles[index2]) = (rectangles[index2], rectangles[index1]);
            LogTextBox.Text += $"Шаг({currentStepIndex + 1}) Сравниваем {height1} и {height2}, {height1} > {height2} Меняем местами\n";
            LogTextBox.ScrollToEnd();
        }
        else
        {
            // Меняем цвет на красный, если не нужно менять местами
            shape1.Fill = Brushes.Red;
            shape2.Fill = Brushes.Red;

            // Ждем некоторое время, чтобы пользователь мог увидеть изменение цвета
            await Task.Delay((int)Math.Round(1000 * (1 / Timeset)));

            // Возвращаем цвет обратно на синий
            shape1.Fill = Brushes.Blue;
            shape2.Fill = Brushes.Blue;
            LogTextBox.Text += $"Шаг({currentStepIndex + 1}) Сравниваем {height1} и {height2}, {height1} < {height2} Остаются на местах\n";
            LogTextBox.ScrollToEnd();
        }
    }
    private async Task Animate(Rectangle shape1, Rectangle shape2, double targetX1, double targetX2)
    {
        double currentX1 = Canvas.GetLeft(shape1);
        double currentX2 = Canvas.GetLeft(shape2);
        int flag = 10;

        while (Math.Abs(currentX1 - targetX1) > 1 || Math.Abs(currentX2 - targetX2) > 1)
        {
            if (Math.Abs(currentX1 - targetX1) > 1)
            {
                currentX1 += (targetX1 - currentX1) / ((int)Math.Round((100 / flag) * (1 / Timeset)));
                Canvas.SetLeft(shape1, currentX1);
            }
            else if (Math.Abs(currentX1 - targetX1) < 1)
            {
                Canvas.SetLeft(shape1, targetX1);
            }
            if (Math.Abs(currentX2 - targetX2) > 1)
            {
                currentX2 += (targetX2 - currentX2) / ((int)Math.Round((100 / flag) * (1 / Timeset)));
                Canvas.SetLeft(shape2, currentX2);
            }
            else if (Math.Abs(currentX2 - targetX2) < 1)
            {
                Canvas.SetLeft(shape2, targetX2);
            }
            await Task.Delay((int)Math.Round((100 / flag) * (1 / Timeset)));
        }
        Canvas.SetLeft(shape1, targetX1);
        Canvas.SetLeft(shape2, targetX2);
    }
}