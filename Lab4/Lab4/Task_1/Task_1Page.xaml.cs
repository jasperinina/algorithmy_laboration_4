using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Lab4.Task_1.Sorting_Algorithms;

namespace Lab4.Task_1
{
    public partial class Task_1Page : Page
    {
        private MainWindow _mainWindow;
        private List<int> numbers;
        private List<(int, int, int)> sortSteps;
        private List<Rectangle> rectangles;
        private List<TextBlock> labels; // Список для меток под столбиками
        private double RectangleWidth;
        private int maxNumber;
        private int currentStepIndex;
        private bool isAnimating;
        private SortingAlgorithm selectedSortingAlgorithm;
        private DataGenerator dataGenerator;
        private double Timeset;

        private StackPanel dynamicPanel;
        private ComboBox SortingAlgorithmComboBox;
        private TextBox arraySizeInputBox; // Поле для ввода размера массива
        private List<string> HeapColors = new List<string>() { "#1F77B4", "#1a689c", "#165782", "#124669", "#0d354f", "#092436", "#05131c" };

        public Task_1Page(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            AddDynamicControls();
            InitializeSortingAlgorithms();
            dataGenerator = new DataGenerator();
            Timeset = 1;

            // Инициализация списков
            numbers = new List<int>();
            sortSteps = new List<(int, int, int)>();
            rectangles = new List<Rectangle>();
            labels = new List<TextBlock>();
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
            SortingAlgorithmComboBox = new ComboBox
            {
                Width = 360,
                Margin = new Thickness(0, 8, 0, 30),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("PopUp")
            };
            SortingAlgorithmComboBox.Items.Add("Пузырьковая сортировка");
            SortingAlgorithmComboBox.Items.Add("Сортировка по выбору");
            SortingAlgorithmComboBox.Items.Add("Быстрая сортировка");
            SortingAlgorithmComboBox.Items.Add("Сортировка по куче");
            SortingAlgorithmComboBox.SelectedIndex = 0;
            SortingAlgorithmComboBox.SelectionChanged += SortingAlgorithmComboBox_SelectionChanged;

            // Заголовок выбора метода сортировки
            TextBlock Header = new TextBlock
            {
                Text = "Введите параметры сортировки",
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("HeaderTextBlockStyle"),
                Margin = new Thickness(0, 0, 0, 0)
            };

            // Поле для ввода количества элементов
            TextBlock arraySizeHeader = new TextBlock
            {
                Text = "Введите количество элементов массива (от 5 до 50)",
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
                Margin = new Thickness(0, 20, 0, 0)
            };

            arraySizeInputBox = new TextBox
            {
                Name = "ArraySizeInput",
                Text = "50", // Значение по умолчанию
                Width = 360,
                Margin = new Thickness(0, 8, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("RoundedTextBoxStyle")
            };

            arraySizeInputBox.LostFocus += (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(arraySizeInputBox.Text) || !int.TryParse(arraySizeInputBox.Text, out int newSize) ||
                    newSize < 5 || newSize > 50)
                {
                    MessageBox.Show("Введите корректное число от 5 до 50.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    arraySizeInputBox.Text = "50"; // Сброс на значение по умолчанию
                }
                else
                {
                    Canvas.Children.Clear(); // Удаление старых прямоугольников и меток
                    InitializeData(newSize); // Генерация нового массива
                }
            };

            // Поле для ввода задержки
            TextBlock delayHeader = new TextBlock
            {
                Text = "Введите задержку визуализации",
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("TextBlockStyle"),
                Margin = new Thickness(0, 0, 0, 8)
            };

            // Ползунок для выбора задержки
            Slider delaySlider = new Slider
            {
                Width = 360,
                Minimum = 0.5,
                Maximum = 5.0,
                Value = 1.0,
                TickFrequency = 0.5,
                IsSnapToTickEnabled = true,
                Margin = new Thickness(0, 0, 0, 10),
                HorizontalAlignment = HorizontalAlignment.Left
            };
            delaySlider.ValueChanged += (sender, e) => { Timeset = delaySlider.Value; };

            // Подписи для чисел на ползунке
            StackPanel sliderLabels = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 20)
            };

            for (double i = 1; i <= 5; i += 1)
            {
                TextBlock label = new TextBlock
                {
                    Text = i.ToString(),
                    Width = 85,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Style = (Style)_mainWindow.FindResource("TextBlockStyle")
                };
                sliderLabels.Children.Add(label);
            }

            // Кнопки "Шаг вперед" и "Шаг назад"
            StackPanel stepsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 140, 0, 20)
            };

            Button nextStepButton = new Button
            {
                Content = "Шаг вперед",
                Width = 170,
                Style = (Style)_mainWindow.FindResource("StepsButtonStyle"),
                Margin = new Thickness(0, 0, 20, 0)
            };
            nextStepButton.Click += NextButton_Click;

            Button backStepButton = new Button
            {
                Content = "Шаг назад",
                Width = 170,
                Style = (Style)_mainWindow.FindResource("StepsButtonStyle")
            };
            backStepButton.Click += BackButton_Click;

            stepsPanel.Children.Add(nextStepButton);
            stepsPanel.Children.Add(backStepButton);

            // Кнопки "Стоп" и "Сбросить"
            StackPanel stopResetPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 0)
            };

            Button stopButton = new Button
            {
                Content = "Стоп",
                Width = 170,
                Style = (Style)_mainWindow.FindResource("RoundedButtonStopStyle"),
                Margin = new Thickness(0, 0, 20, 0)
            };
            stopButton.Click += StopButton_Click;

            Button resetButton = new Button
            {
                Content = "Сбросить",
                Width = 170,
                Style = (Style)_mainWindow.FindResource("RoundedButtonStopStyle")
            };
            resetButton.Click += ResetButton_Click;

            stopResetPanel.Children.Add(stopButton);
            stopResetPanel.Children.Add(resetButton);

            // Кнопка "Запустить сортировку"
            Button startButton = new Button
            {
                Content = "Запустить сортировку",
                Width = 360,
                Margin = new Thickness(0, 40, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = (Style)_mainWindow.FindResource("RoundedButtonStyle")
            };
            startButton.Click += StartButton_Click;

            // Добавление элементов на панель
            dynamicPanel.Children.Add(sortMethodHeader);      // 0
            dynamicPanel.Children.Add(SortingAlgorithmComboBox); // 1
            dynamicPanel.Children.Add(Header);                 // 2
            dynamicPanel.Children.Add(arraySizeHeader);        // 3
            dynamicPanel.Children.Add(arraySizeInputBox);      // 4
            dynamicPanel.Children.Add(delayHeader);            // 5
            dynamicPanel.Children.Add(delaySlider);            // 6
            dynamicPanel.Children.Add(sliderLabels);           // 7
            dynamicPanel.Children.Add(stepsPanel);             // 8
            dynamicPanel.Children.Add(stopResetPanel);         // 9
            dynamicPanel.Children.Add(startButton);            // 10

            _mainWindow.PageContentControl.Content = dynamicPanel;
        }

        private void InitializeSortingAlgorithms()
        {
            selectedSortingAlgorithm = new BubbleSort();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.Children.Clear(); // Очистка канвы
            InitializeData(50); // Инициализация массива с 50 элементами по умолчанию
            DrawRectangles();
        }

        private void InitializeData(int arraySize)
        {
            // Очистка данных и канвы перед инициализацией
            numbers = dataGenerator.ArrayGenerate(arraySize, 50).ToList(); // Генерация нового массива
            sortSteps = new List<(int, int, int)>();
            rectangles = new List<Rectangle>();
            labels = new List<TextBlock>(); // Инициализация списка меток
            selectedSortingAlgorithm.Sort(numbers.ToArray(), sortSteps); // Создание шагов сортировки
            maxNumber = numbers.Max();
            RectangleWidth = Canvas.ActualWidth / numbers.Count;

            // Очистка Canvas перед отрисовкой
            Canvas.Children.Clear();
            DrawRectangles(); // Отрисовка новых прямоугольников и меток
        }

        private void DrawRectangles()
        {
            double Spacing = 0;
            double canvasHeight = Canvas.ActualHeight;

            for (int i = 0; i < numbers.Count; i++)
            {
                int number = numbers[i];
                double height = (number) * canvasHeight * 0.95 / maxNumber;

                // Создание столбика
                Rectangle rect = new Rectangle
                {
                    Width = RectangleWidth,
                    Height = height,
                    Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1F77B4")), // Синий цвет
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };

                Canvas.SetLeft(rect, Spacing);
                Canvas.SetBottom(rect, 20); // Отступ снизу для метки
                Canvas.Children.Add(rect);
                rectangles.Add(rect);

                // Создание метки
                TextBlock label = new TextBlock
                {
                    Text = number.ToString(),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Width = RectangleWidth,
                    TextWrapping = TextWrapping.Wrap,
                    Style = (Style)_mainWindow.FindResource("TextBlockStyle")
                };

                // Размещение метки под столбиком
                Canvas.SetLeft(label, Spacing);
                Canvas.SetBottom(label, 0); // Метка размещается непосредственно под столбиком
                Canvas.Children.Add(label);
                labels.Add(label);

                Spacing += RectangleWidth;
            }
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimating)
            {
                isAnimating = true;

                if (selectedSortingAlgorithm is HeapSort)
                {
                    await ColorRectanglesByHeapLevels();
                }
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
                var step = sortSteps[currentStepIndex];
                Rectangle rect1 = rectangles[step.Item1];
                Rectangle rect2 = rectangles[step.Item2];
                TextBlock label1 = labels[step.Item1];
                TextBlock label2 = labels[step.Item2];
                await SwapElements(rect1, label1, rect2, label2, step.Item3);
                currentStepIndex++;
            }
        }

        private async void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!isAnimating && currentStepIndex > 0)
            {
                currentStepIndex--;
                var step = sortSteps[currentStepIndex];
                Rectangle rect1 = rectangles[step.Item2];
                Rectangle rect2 = rectangles[step.Item1];
                TextBlock label1 = labels[step.Item2];
                TextBlock label2 = labels[step.Item1];
                await SwapElements(rect1, label1, rect2, label2, step.Item3);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            isAnimating = false;
            currentStepIndex = 0;

            // Очистка данных и канвы
            numbers.Clear();
            rectangles.Clear();
            labels.Clear();
            Canvas.Children.Clear();

            // Получение размера массива из текстового поля
            if (!int.TryParse(arraySizeInputBox.Text, out int arraySize) || arraySize < 5 || arraySize > 50)
            {
                MessageBox.Show("Некорректный размер массива. Установлено значение по умолчанию: 50.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                arraySize = 50; // Значение по умолчанию
            }

            // Инициализация данных с новым размером
            InitializeData(arraySize);
            LogTextBox.Text = string.Empty;
        }

        private void SortingAlgorithmComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedAlgorithm = (string)SortingAlgorithmComboBox.SelectedItem;

            switch (selectedAlgorithm)
            {
                case "Пузырьковая сортировка":
                    selectedSortingAlgorithm = new BubbleSort();
                    break;
                case "Сортировка по выбору":
                    selectedSortingAlgorithm = new SelectionSort();
                    break;
                case "Быстрая сортировка":
                    selectedSortingAlgorithm = new QuickSort();
                    break;
                case "Сортировка по куче":
                    selectedSortingAlgorithm = new HeapSort();
                    break;
                default:
                    selectedSortingAlgorithm = new BubbleSort();
                    break;
            }

            // Очистка канвы и инициализация данных с текущей длиной массива
            if (numbers != null && numbers.Count > 0)
            {
                Canvas.Children.Clear();
                InitializeData(numbers.Count);
            }
        }

        private async Task ContinueAnimation()
        {
            while (isAnimating && currentStepIndex < sortSteps.Count)
            {
                var step = sortSteps[currentStepIndex];
                Rectangle rect1 = rectangles[step.Item1];
                Rectangle rect2 = rectangles[step.Item2];
                TextBlock label1 = labels[step.Item1];
                TextBlock label2 = labels[step.Item2];
                await SwapElements(rect1, label1, rect2, label2, step.Item3);
                currentStepIndex++;
            }

            isAnimating = false;
        }
        private async Task ColorRectanglesByHeapLevels()
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                int level = (int)Math.Floor(Math.Log(i + 1, 2));
                rectangles[i].Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(HeapColors[level]));
            }
            LogTextBox.Text += $"Разбиваем массив по индексам\n";
            LogTextBox.ScrollToEnd();
            await Task.Delay((int)Math.Round(1000 * (1 / Timeset)));
        }

        private async Task SwapElements(Rectangle shape1, TextBlock label1, Rectangle shape2, TextBlock label2, int shouldSwap)
        {
            var color1 = shape1.Fill;
            var color2 = shape2.Fill;
            int index1 = rectangles.IndexOf(shape1);
            int index2 = rectangles.IndexOf(shape2);
            double shape1X = Canvas.GetLeft(shape1);
            double shape2X = Canvas.GetLeft(shape2);

            switch (shouldSwap)
            {
                case 1: // BubbleSort : true
                    // Меняем цвет на зеленый перед анимацией (#1FB451)
                    shape1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1FB451"));
                    shape2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1FB451"));

                    // Анимируем перемещение shape1 и label1 на место shape2 и label2, и наоборот
                    await AnimateSwap(shape1, label1, shape2, label2, shape2X, shape1X);

                    // Обмен элементов в списках
                    if (index1 != -1 && index2 != -1)
                    {
                        (rectangles[index1], rectangles[index2]) = (rectangles[index2], rectangles[index1]);
                        (labels[index1], labels[index2]) = (labels[index2], labels[index1]);
                    }

                    // Возвращаем цвет обратно на синий после анимации (#1F77B4)
                    shape1.Fill = color2;
                    shape2.Fill = color1;

                    LogTextBox.Text +=
                        $"Шаг {currentStepIndex + 1}\n\n[Сравнение]\n    • {labels[index2].Text} и {labels[index1].Text}\n    • {labels[index2].Text} > {labels[index1].Text}\n\nМеняем местами\n\n";
                    LogTextBox.ScrollToEnd();
                    break;
                case 2: // Heapsort : Max -> toend
                    LogTextBox.Text += $"Шаг {currentStepIndex + 1} На данном этапе самый большой элемент {labels[index1].Text} хранится в корне кучи. Замените его на последний элемент кучи {labels[index2].Text}\n Поднимаем наибольший элемент снизу пирамиды";
                    LogTextBox.ScrollToEnd();
                    shape1.Fill = Brushes.Green;
                    shape2.Fill = Brushes.Green;

                    await AnimateSwap(shape2, label2, shape1, label1, shape1X, shape2X);

                    shape1.Fill = color2;
                    shape2.Fill = color1;

                    if (index1 != -1 && index2 != -1)
                    {
                        (rectangles[index1], rectangles[index2]) = (rectangles[index2], rectangles[index1]);
                        (labels[index1], labels[index2]) = (labels[index2], labels[index1]);
                    }
                    break;
                case 3: // Heapsort : true
                    LogTextBox.Text += $"Шаг {currentStepIndex + 1} Сравниваем {labels[index1].Text} и его родителя {labels[index2].Text} с индексом (i-1)/2, {labels[index1].Text} > {labels[index2].Text} Меняем местами\n";
                    LogTextBox.ScrollToEnd();
                    shape1.Fill = Brushes.Green;
                    shape2.Fill = Brushes.Green;

                    await AnimateSwap(shape1, label1, shape2, label2, shape2X, shape1X);

                    shape1.Fill = color2;
                    shape2.Fill = color1;

                    if (index1 != -1 && index2 != -1)
                    {
                        (rectangles[index1], rectangles[index2]) = (rectangles[index2], rectangles[index1]);
                        (labels[index1], labels[index2]) = (labels[index2], labels[index1]);
                    }
                    break;
                case 4: // Heapsort : false
                    LogTextBox.Text += $"Шаг {currentStepIndex + 1} Сравниваем {labels[index1].Text} и его родителя {labels[index2].Text} с индексом (i-1)/2, {labels[index1].Text} > {labels[index2].Text} Остаются на местах\n";
                    LogTextBox.ScrollToEnd();
                    shape1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B41F1F"));
                    shape2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B41F1F"));

                    await Task.Delay((int)Math.Round(1000 * (1 / Timeset)));

                    shape1.Fill = color1;
                    shape2.Fill = color2;
                    break;
                default: // BubbleSort : false
                    // Меняем цвет на красный, если не нужно менять местами (#B41F1F)
                    shape1.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B41F1F"));
                    shape2.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B41F1F"));

                    // Ждем некоторое время, чтобы пользователь мог увидеть изменение цвета
                    await Task.Delay((int)Math.Round(1000 * (1 / Timeset)));

                    // Возвращаем цвет обратно на синий (#1F77B4)
                    shape1.Fill = color1;
                    shape2.Fill = color2;
                    LogTextBox.Text +=
                        $"Шаг {currentStepIndex + 1}\n\n[Сравнение]\n    • {labels[index1].Text} и {labels[index2].Text}\n    • {labels[index1].Text} < {labels[index2].Text}\n\nОстаются на местах\n\n";
                    LogTextBox.ScrollToEnd();
                    break;
            }
        }  

        private async Task AnimateSwap(Rectangle rect1, TextBlock label1, Rectangle rect2, TextBlock label2, double targetX1, double targetX2)
        {
            double currentX1 = Canvas.GetLeft(rect1);
            double currentX2 = Canvas.GetLeft(rect2);
            int flag = 10;

            while (Math.Abs(currentX1 - targetX1) > 1 || Math.Abs(currentX2 - targetX2) > 1 ||
                   Math.Abs(Canvas.GetLeft(label1) - targetX1) > 1 || Math.Abs(Canvas.GetLeft(label2) - targetX2) > 1)
            {
                if (Math.Abs(currentX1 - targetX1) > 1)
                {
                    currentX1 += (targetX1 - currentX1) / ((int)Math.Round((100 / flag) * (1 / Timeset)));
                    Canvas.SetLeft(rect1, currentX1);
                    Canvas.SetLeft(label1, currentX1); // Перемещение метки вместе со столбиком
                }
                else
                {
                    Canvas.SetLeft(rect1, targetX1);
                    Canvas.SetLeft(label1, targetX1);
                }

                if (Math.Abs(currentX2 - targetX2) > 1)
                {
                    currentX2 += (targetX2 - currentX2) / ((int)Math.Round((100 / flag) * (1 / Timeset)));
                    Canvas.SetLeft(rect2, currentX2);
                    Canvas.SetLeft(label2, currentX2); // Перемещение метки вместе со столбиком
                }
                else
                {
                    Canvas.SetLeft(rect2, targetX2);
                    Canvas.SetLeft(label2, targetX2);
                }

                await Task.Delay((int)Math.Round((100 / flag) * (1 / Timeset)));
            }

            Canvas.SetLeft(rect1, targetX1);
            Canvas.SetLeft(label1, targetX1);
            Canvas.SetLeft(rect2, targetX2);
            Canvas.SetLeft(label2, targetX2);
        }
    }
}