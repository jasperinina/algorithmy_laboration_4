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
        
        public void Sort(string sortMethod, int primaryKeyIndex, Action<string> visualizeStep, int delay)
        {
            var lines = _fileHandler.ReadFromFile();

            if (lines == null || lines.Count == 0)
            {
                visualizeStep("Файл пуст.");
                return;
            }

            var headers = lines[0]; // Первая строка — заголовок
            var allColumns = headers.Split(',').Select(h => h.Trim()).ToArray();

            // Фильтруем строки, у которых количество столбцов больше индекса основного атрибута
            var data = lines.Skip(1)
                            .Where(line => line.Split(',').Length > primaryKeyIndex)
                            .ToList();

            if (data.Count == 0)
            {
                visualizeStep("Нет данных для сортировки после фильтрации.");
                return;
            }

            _stepNumber = 0; // Сброс счетчика шагов

            List<string> sortedData = sortMethod switch
            {
                "Прямое слияние" => DirectMergeSort(data, primaryKeyIndex, visualizeStep, delay),
                "Естественное слияние" => NaturalMergeSort(data, primaryKeyIndex, visualizeStep, delay),
                "Многопутевое слияние" => MultiwayMergeSort(data, primaryKeyIndex, visualizeStep, delay),
                _ => throw new NotImplementedException("Выбранный метод сортировки не поддерживается.")
            };

            // Записываем результат в файл
            var sortedLines = new List<string> { headers }.Concat(sortedData).ToList();
            _fileHandler.WriteToFile(sortedLines);
        }

        /// <summary>
        /// Реализация прямого слияния (Direct Merge Sort).
        /// </summary>
        private List<string> DirectMergeSort(
            List<string> data,
            int primaryKeyIndex,
            Action<string> visualizeStep,
            int delay)
        {
            var sortedBlocks = SplitIntoSortedBlocks(data, primaryKeyIndex);

            while (sortedBlocks.Count > 1)
            {
                var mergedBlocks = new List<List<string>>();

                for (int i = 0; i < sortedBlocks.Count; i += 2)
                {
                    if (i + 1 < sortedBlocks.Count)
                    {
                        var block1 = sortedBlocks[i];
                        var block2 = sortedBlocks[i + 1];
                        var merged = Merge(block1, block2, primaryKeyIndex, visualizeStep);

                        mergedBlocks.Add(merged);
                    }
                    else
                    {
                        mergedBlocks.Add(sortedBlocks[i]);
                    }
                }

                sortedBlocks = mergedBlocks;

                // Визуализация текущего шага
                var description = "Текущий результат: " + string.Join(", ", sortedBlocks.SelectMany(b => b));
                _stepNumber++;
                visualizeStep($"Шаг {_stepNumber}:\n{description}");
                Thread.Sleep(delay);
            }

            return sortedBlocks[0];
        }

        /// <summary>
        /// Реализация естественного слияния (Natural Merge Sort).
        /// </summary>
        private List<string> NaturalMergeSort(
            List<string> data,
            int primaryKeyIndex,
            Action<string> visualizeStep,
            int delay)
        {
            var runs = FindNaturalRuns(data, primaryKeyIndex);

            while (runs.Count > 1)
            {
                var mergedRuns = new List<List<string>>();

                for (int i = 0; i < runs.Count; i += 2)
                {
                    if (i + 1 < runs.Count)
                    {
                        var run1 = runs[i];
                        var run2 = runs[i + 1];
                        var merged = Merge(run1, run2, primaryKeyIndex, visualizeStep);

                        mergedRuns.Add(merged);
                    }
                    else
                    {
                        mergedRuns.Add(runs[i]);
                    }
                }

                runs = mergedRuns;

                // Визуализация текущего шага
                var description = "Текущий результат: " + string.Join(", ", runs.SelectMany(r => r));
                _stepNumber++;
                visualizeStep($"Шаг {_stepNumber}:\n{description}");
                Thread.Sleep(delay);
            }

            return runs.SelectMany(r => r).ToList();
        }

        /// <summary>
        /// Реализация многопутевого слияния (Multiway Merge Sort).
        /// </summary>
        private List<string> MultiwayMergeSort(
            List<string> data,
            int primaryKeyIndex,
            Action<string> visualizeStep,
            int delay)
        {
            var runs = SplitIntoSortedBlocks(data, primaryKeyIndex);

            while (runs.Count > 1)
            {
                var mergedRuns = MergeMultiway(runs, primaryKeyIndex, visualizeStep);

                // Визуализация текущего шага
                var description = "Текущий результат: " + string.Join(", ", mergedRuns);
                _stepNumber++;
                visualizeStep($"Шаг {_stepNumber}:\n{description}");
                Thread.Sleep(delay);

                runs = SplitIntoSortedBlocks(mergedRuns, primaryKeyIndex);
            }

            return runs.SelectMany(r => r).ToList();
        }

        /// <summary>
        /// Слияние двух блоков данных.
        /// </summary>
        private List<string> Merge(
            List<string> block1,
            List<string> block2,
            int primaryKeyIndex,
            Action<string> visualizeStep)
        {
            int i = 0, j = 0;
            var merged = new List<string>();

            while (i < block1.Count && j < block2.Count)
            {
                var element1 = block1[i];
                var element2 = block2[j];

                var primaryKey1 = GetKey(element1, primaryKeyIndex);
                var primaryKey2 = GetKey(element2, primaryKeyIndex);

                string action;

                if (string.Compare(primaryKey1, primaryKey2) < 0)
                {
                    merged.Add(element1);
                    action = "Оставляем на месте";
                    i++;
                }
                else
                {
                    merged.Add(element2);
                    action = "Перемещаем элемент";
                    j++;
                }

                _stepNumber++;
                var description = $"Шаг {_stepNumber}:\nСравниваем элементы:\n- {element1}\n- {element2}\nДействие: {action}.";
                visualizeStep(description);
                Thread.Sleep(500); // Задержка для визуализации
            }

            // Обработка оставшихся элементов из block1
            while (i < block1.Count)
            {
                var element = block1[i];
                merged.Add(element);
                i++;

                _stepNumber++;
                var description = $"Шаг {_stepNumber}:\nДобавляем оставшийся элемент: {element}.";
                visualizeStep(description);
                Thread.Sleep(500);
            }

            // Обработка оставшихся элементов из block2
            while (j < block2.Count)
            {
                var element = block2[j];
                merged.Add(element);
                j++;

                _stepNumber++;
                var description = $"Шаг {_stepNumber}:\nДобавляем оставшийся элемент: {element}.";
                visualizeStep(description);
                Thread.Sleep(500);
            }

            return merged;
        }

        /// <summary>
        /// Многопутевое слияние нескольких блоков данных.
        /// </summary>
        private List<string> MergeMultiway(
            List<List<string>> blocks,
            int primaryKeyIndex,
            Action<string> visualizeStep)
        {
            var sortedData = new List<string>();

            // Используем SortedSet для упорядочивания элементов по ключу
            var heap = new SortedSet<(string Key, int BlockIndex, int ElementIndex)>();

            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].Count > 0)
                {
                    var key = GetKey(blocks[i][0], primaryKeyIndex);
                    heap.Add((key, i, 0));
                }
            }

            while (heap.Count > 0)
            {
                var min = heap.Min;
                heap.Remove(min);

                var element = blocks[min.BlockIndex][min.ElementIndex];
                sortedData.Add(element);

                // Визуализация добавления элемента
                _stepNumber++;
                var description = $"Шаг {_stepNumber}:\nДобавляем элемент: {element}.";
                visualizeStep(description);
                Thread.Sleep(500);

                int nextIndex = min.ElementIndex + 1;
                if (nextIndex < blocks[min.BlockIndex].Count)
                {
                    var key = GetKey(blocks[min.BlockIndex][nextIndex], primaryKeyIndex);
                    heap.Add((key, min.BlockIndex, nextIndex));
                }
            }

            return sortedData;
        }

        /// <summary>
        /// Разбиение данных на отсортированные блоки.
        /// </summary>
        private List<List<string>> SplitIntoSortedBlocks(List<string> data, int primaryKeyIndex)
        {
            return data.Select(d => new List<string> { d })
                       .OrderBy(b => GetKey(b[0], primaryKeyIndex))
                       .ToList();
        }

        /// <summary>
        /// Нахождение естественных "ран" в данных для естественного слияния.
        /// </summary>
        private List<List<string>> FindNaturalRuns(List<string> data, int primaryKeyIndex)
        {
            var runs = new List<List<string>>();
            var currentRun = new List<string> { data[0] };

            for (int i = 1; i < data.Count; i++)
            {
                var currentKey = GetKey(data[i], primaryKeyIndex);
                var previousKey = GetKey(data[i - 1], primaryKeyIndex);

                if (string.Compare(currentKey, previousKey) >= 0)
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