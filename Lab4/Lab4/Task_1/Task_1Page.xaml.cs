using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Threading.Tasks;
using Lab4.Task_1.Sorting_Algorithms;

namespace Lab4.Task_1;

public partial class Task_1Page : Page
{
    private MainWindow _mainWindow;
    private List<int> numbers;
    private List<(int, int)> sortSteps;
    private List<Rectangle> rectangles;
    private double RectangleWidth;
    private int maxNumber;
    private int currentStepIndex;
    private bool isAnimating;
    private SortingAlgorithm selectedSortingAlgorithm;
    private DataGenerator dataGenerator;

    public Task_1Page(MainWindow mainWindow)
    {
        InitializeComponent();
        _mainWindow = mainWindow;
        InitializeComponent();
        InitializeSortingAlgorithms();
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
        numbers = new List<int>() { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
        sortSteps = new List<(int, int)>();
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
            double height = number * Canvas.ActualHeight * 0.95 / maxNumber;

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
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item1], rectangles[sortSteps[currentStepIndex].Item2]);
            currentStepIndex++;
        }
    }
    private async void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (!isAnimating && currentStepIndex > 0)
        {
            currentStepIndex--;
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item2], rectangles[sortSteps[currentStepIndex].Item1]);
        }
    }
    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        isAnimating = false;
        currentStepIndex = 0;
        Canvas.Children.Clear();
        InitializeData();
        DrawRectangles();
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
            await SwapElements(rectangles[sortSteps[currentStepIndex].Item1], rectangles[sortSteps[currentStepIndex].Item2]);
            currentStepIndex++;
        }
        isAnimating = false;
    }
    private async Task SwapElements(Rectangle shape1, Rectangle shape2)
    {
        double shape1X = Canvas.GetLeft(shape1);
        double shape1Y = Canvas.GetTop(shape1);
        double shape2X = Canvas.GetLeft(shape2);
        double shape2Y = Canvas.GetTop(shape2);

        await Animate(shape1, shape2X, shape2Y);
        await Animate(shape2, shape1X, shape1Y);
        int index1 = rectangles.IndexOf(shape1);
        int index2 = rectangles.IndexOf(shape2);
        (rectangles[index1], rectangles[index2]) = (rectangles[index2], rectangles[index1]);
    }
    private async Task Animate(Rectangle shape, double targetX, double targetY)
    {
        double currentX = Canvas.GetLeft(shape);
        double currentY = Canvas.GetTop(shape);
        int flag = 60;

        while (Math.Abs(currentX - targetX) > 1 || Math.Abs(currentY - targetY) > 1)
        {
            if (Math.Abs(currentX - targetX) > 1)
            {
                currentX += (targetX - currentX) / (100.0 / flag);
                Canvas.SetLeft(shape, currentX);
            }
            if (Math.Abs(currentY - targetY) > 1)
            {
                currentY += (targetY - currentY) / (100.0 / flag);
                Canvas.SetTop(shape, currentY);
            }
            if (currentX < 0)
            {
                currentX = 0;
                Canvas.SetLeft(shape, currentX);
            }
            if (currentX + shape.Width > Canvas.ActualWidth)
            {
                currentX = Canvas.ActualWidth - shape.Width;
                Canvas.SetLeft(shape, currentX);
            }
            if (currentY < 0)
            {
                currentY = 0;
                Canvas.SetTop(shape, currentY);
            }
            if (currentY + shape.Height > Canvas.ActualHeight)
            {
                currentY = Canvas.ActualHeight - shape.Height;
                Canvas.SetTop(shape, currentY);
            }
            await Task.Delay(100 / flag);
        }
        Canvas.SetLeft(shape, targetX);
        Canvas.SetTop(shape, targetY);
    }
}