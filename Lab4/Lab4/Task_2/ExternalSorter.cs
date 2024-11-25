using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Lab4.Task_2
{
    public class ExternalSorter
    {
        private readonly FileHandler _fileHandler;
        private int _stepNumber; // Глобальный счетчик шагов

        public ExternalSorter(FileHandler fileHandler)
        {
            _fileHandler = fileHandler;
            _stepNumber = 0;
        }

        /// <summary>
        /// Выполняет сортировку данных.
        /// </summary>
        /// <param name="sortMethod">Метод сортировки.</param>
        /// <param name="filterKeyIndex">Индекс атрибута фильтрации.</param>
        /// <param name="filterKeyValue">Значение для фильтрации.</param>
        /// <param name="secondaryKeyIndex">Индекс вторичного атрибута сортировки.</param>
        /// <param name="progress">Объект IProgress для отчетов о прогрессе.</param>
        /// <param name="delay">Задержка между шагами в миллисекундах.</param>
        public void Sort(
            string sortMethod,
            int filterKeyIndex,
            string filterKeyValue,
            int secondaryKeyIndex,
            IProgress<string> progress,
            int delay)
        {
            // Чтение данных из файла
            var data = _fileHandler.ReadFromFile();

            if (data == null || data.Count == 0)
            {
                progress?.Report("Файл пуст.");
                return;
            }

            var headers = data[0]; // Первая строка — заголовок
            var allColumns = headers.Split(',').Select(h => h.Trim()).ToArray();

            // Фильтрация строк, которые содержат значение атрибута фильтрации
            var filteredData = data.Skip(1).Where(line => GetKey(line, filterKeyIndex) == filterKeyValue).ToList();

            // Отправка сообщения о фильтрации
            progress?.Report($"Оставляем элементы с выбранным ключем \"{filterKeyValue}\".");
            progress?.Report($"Фильтрованные данные:\n{string.Join("\n", filteredData)}\n");

            if (filteredData.Count == 0)
            {
                progress?.Report($"Нет данных для сортировки по значению \"{filterKeyValue}\".");
                return;
            }

            // Получение названия вторичного ключа
            var secondaryKeyName = allColumns[secondaryKeyIndex];

            // Выполнение выбранного метода сортировки
            List<string> sortedData = sortMethod switch
            {
                "Прямое слияние" => DirectMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress, delay),
                "Естественное слияние" => NaturalMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress, delay),
                "Многопутевое слияние" => MultiwayMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress, delay),
                _ => throw new NotSupportedException($"Метод сортировки {sortMethod} не поддерживается.")
            };

            // Добавляем заголовок обратно в результат
            var result = new List<string> { headers }.Concat(sortedData).ToList();

            // Запись результата в файл
            _fileHandler.WriteToFile(result);

            // Отправка сообщения о завершении сортировки
            progress?.Report($"Все строки упорядочены по ключу \"{secondaryKeyName}\".\n");
        }

        private List<string> DirectMergeSort(
            List<string> data,
            int secondaryKeyIndex,
            string secondaryKeyName,
            IProgress<string> progress,
            int delay)
        {
            // Разбиение данных на отдельные блоки
            var sortedBlocks = SplitIntoSortedBlocks(data, secondaryKeyIndex);
            progress?.Report($"Разбиение:\nКаждый элемент становится отдельным блоком.\nНачальное состояние:\n" +
                            $"{string.Join("\n", sortedBlocks.Select(b => $"[{b[0]}]"))}");

            int step = 1;
            while (sortedBlocks.Count > 1)
            {
                var mergedBlocks = new List<List<string>>();
                progress?.Report($"Слияние пар блоков: Шаг {step}");

                for (int i = 0; i < sortedBlocks.Count; i += 2)
                {
                    if (i + 1 < sortedBlocks.Count)
                    {
                        var block1 = sortedBlocks[i];
                        var block2 = sortedBlocks[i + 1];
                        var merged = Merge(block1, block2, secondaryKeyIndex, progress);
                        mergedBlocks.Add(merged);

                        progress?.Report(
                            $"Слияние блоков:\n{string.Join(", ", block1)} и {string.Join(", ", block2)}.\nРезультат: {string.Join(", ", merged)}");
                    }
                    else
                    {
                        mergedBlocks.Add(sortedBlocks[i]);
                        progress?.Report($"Блок \"{string.Join(", ", sortedBlocks[i])}\" остается без изменений.");
                    }
                }

                sortedBlocks = mergedBlocks;
                progress?.Report($"После шага {step}: осталось {sortedBlocks.Count} блоков.\n");
                step++;
                Thread.Sleep(delay);
            }

            progress?.Report($"Все строки упорядочены по ключу \"{secondaryKeyName}\". Результат:\n{string.Join("\n", sortedBlocks[0])}");
            return sortedBlocks[0];
        }

        private List<string> NaturalMergeSort(
            List<string> data,
            int secondaryKeyIndex,
            string secondaryKeyName,
            IProgress<string> progress,
            int delay)
        {
            // Нахождение естественных последовательностей
            var runs = FindNaturalRuns(data, secondaryKeyIndex);
            progress?.Report(
                $"Разбиение на естественные последовательности:\nНайдено {runs.Count} последовательностей:\n" +
                $"{string.Join("\n", runs.Select(r => $"[{string.Join(", ", r)}]"))}");

            int step = 1;
            while (runs.Count > 1)
            {
                var mergedRuns = new List<List<string>>();
                progress?.Report($"Слияние последовательностей: Шаг {step}");

                for (int i = 0; i < runs.Count; i += 2)
                {
                    if (i + 1 < runs.Count)
                    {
                        var run1 = runs[i];
                        var run2 = runs[i + 1];
                        var merged = Merge(run1, run2, secondaryKeyIndex, progress);
                        mergedRuns.Add(merged);

                        progress?.Report(
                            $"Слияние последовательностей:\n{string.Join(", ", run1)} и {string.Join(", ", run2)}.\nРезультат: {string.Join(", ", merged)}");
                    }
                    else
                    {
                        mergedRuns.Add(runs[i]);
                        progress?.Report($"Последовательность \"{string.Join(", ", runs[i])}\" остается без изменений.");
                    }
                }

                runs = mergedRuns;
                progress?.Report($"После шага {step}: осталось {runs.Count} последовательностей.\n");
                step++;
                Thread.Sleep(delay);
            }

            progress?.Report($"Все строки упорядочены по ключу \"{secondaryKeyName}\". Результат:\n{string.Join("\n", runs.SelectMany(r => r))}");
            return runs.SelectMany(r => r).ToList();
        }

        private List<string> MultiwayMergeSort(
            List<string> data,
            int secondaryKeyIndex,
            string secondaryKeyName,
            IProgress<string> progress,
            int delay)
        {
            // Разбиение данных на блоки
            var runs = SplitIntoSortedBlocks(data, secondaryKeyIndex);
            progress?.Report($"Начало многопутевого слияния:\nКоличество блоков: {runs.Count}\n" +
                            $"{string.Join("\n", runs.Select((b, index) => $"Блок {index + 1}: [{string.Join(", ", b)}]"))}");

            var sortedData = new List<string>();
            var heap = new PriorityQueue<(string Key, int BlockIndex, int ElementIndex), string>();

            // Инициализация PriorityQueue минимальными элементами каждого блока
            for (int i = 0; i < runs.Count; i++)
            {
                if (runs[i].Count > 0)
                {
                    var key = GetKey(runs[i][0], secondaryKeyIndex);
                    heap.Enqueue((key, i, 0), key);
                }
            }

            progress?.Report($"Инициализация завершена: добавлено {heap.Count} минимальных элементов в очередь.");

            // Многопутевое слияние
            while (heap.Count > 0)
            {
                var min = heap.Dequeue();

                var element = runs[min.BlockIndex][min.ElementIndex];
                sortedData.Add(element);

                progress?.Report($"Шаг {_stepNumber}: добавлен элемент \"{element}\" из блока {min.BlockIndex + 1}.");

                int nextIndex = min.ElementIndex + 1;
                if (nextIndex < runs[min.BlockIndex].Count)
                {
                    var key = GetKey(runs[min.BlockIndex][nextIndex], secondaryKeyIndex);
                    heap.Enqueue((key, min.BlockIndex, nextIndex), key);
                }

                _stepNumber++;
                Thread.Sleep(delay);
            }

            progress?.Report($"Все строки упорядочены по ключу \"{secondaryKeyName}\". Результат:\n{string.Join("\n", sortedData)}");
            return sortedData;
        }

        private List<string> Merge(
            List<string> block1,
            List<string> block2,
            int secondaryKeyIndex,
            IProgress<string> progress)
        {
            int i = 0, j = 0;
            var merged = new List<string>();

            while (i < block1.Count && j < block2.Count)
            {
                var key1 = GetKey(block1[i], secondaryKeyIndex);
                var key2 = GetKey(block2[j], secondaryKeyIndex);

                if (string.Compare(key1, key2, StringComparison.Ordinal) < 0)
                {
                    merged.Add(block1[i]);
                    progress?.Report($"Сравнение ключей: {key1} < {key2}. Добавляем \"{block1[i]}\".");
                    i++;
                }
                else
                {
                    merged.Add(block2[j]);
                    progress?.Report($"Сравнение ключей: {key1} >= {key2}. Добавляем \"{block2[j]}\".");
                    j++;
                }
            }

            while (i < block1.Count)
            {
                merged.Add(block1[i]);
                i++;
            }

            while (j < block2.Count)
            {
                merged.Add(block2[j]);
                j++;
            }

            return merged;
        }

        private List<List<string>> SplitIntoSortedBlocks(List<string> data, int secondaryKeyIndex)
        {
            // Удаляем сортировку, чтобы сохранить исходный порядок
            return data.Select(d => new List<string> { d }).ToList();
        }

        private List<List<string>> FindNaturalRuns(List<string> data, int secondaryKeyIndex)
        {
            var runs = new List<List<string>>();
            var currentRun = new List<string> { data[0] };

            for (int i = 1; i < data.Count; i++)
            {
                var currentKey = GetKey(data[i], secondaryKeyIndex);
                var previousKey = GetKey(data[i - 1], secondaryKeyIndex);

                if (string.Compare(currentKey, previousKey, StringComparison.Ordinal) >= 0)
                {
                    currentRun.Add(data[i]);
                }
                else
                {
                    runs.Add(currentRun);
                    currentRun = new List<string> { data[i] };
                }
            }

            if (currentRun.Count > 0)
                runs.Add(currentRun);

            return runs;
        }

        /// <summary>
        /// Извлечение ключа из строки по указанному индексу.
        /// </summary>
        private string GetKey(string line, int keyIndex)
        {
            var parts = line.Split(',');
            if (keyIndex < 0 || keyIndex >= parts.Length)
                throw new IndexOutOfRangeException("Указанный индекс ключа выходит за пределы таблицы.");

            return parts[keyIndex].Trim();
        }
    }
}
