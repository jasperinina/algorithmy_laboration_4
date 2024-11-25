namespace Lab4.Task_2
{
    public class ExternalSorter
    {
        private readonly FileHandler _fileHandler;
        private int _stepNumber;

        public ExternalSorter(FileHandler fileHandler)
        {
            _fileHandler = fileHandler;
            _stepNumber = 0;
        }

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

            progress?.Report($"[Фильтрация данных]\n\nОставляем элементы с выбранным ключем \"{filterKeyValue}\".");
            progress?.Report($"Фильтрованные данные:\n\n    • {string.Join("\n\n    • ", filteredData)}\n");

            if (filteredData.Count == 0)
            {
                progress?.Report($"Нет данных для сортировки по значению \"{filterKeyValue}\".");
                return;
            }

            // Получение названия вторичного ключа
            var secondaryKeyName = allColumns[secondaryKeyIndex];

            List<string> sortedData = sortMethod switch
            {
                "Прямое слияние" => DirectMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress, delay),
                "Естественное слияние" => NaturalMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress,
                    delay),
                "Многопутевое слияние" => MultiwayMergeSort(filteredData, secondaryKeyIndex, secondaryKeyName, progress,
                    delay),
                _ => throw new NotSupportedException($"Метод сортировки \"{sortMethod}\" не поддерживается.")
            };

            // Добавляем заголовок обратно в результат
            var result = new List<string> { headers }.Concat(sortedData).ToList();

            // Запись результата в файл
            _fileHandler.WriteToFile(result);
        }

        private List<string> DirectMergeSort(List<string> data, int secondaryKeyIndex, string secondaryKeyName, IProgress<string> progress, int delay)
        {
            // Разбиение данных на отдельные блоки (каждый элемент — отдельный блок)
            var sortedBlocks = SplitIntoSortedBlocks(data, secondaryKeyIndex);
            progress?.Report(
                $"[Разбиение данных]\n\nКаждый элемент становится отдельным блоком.\n\nНачальное состояние блоков:");
            foreach (var block in sortedBlocks)
            {
                progress?.Report($"   • {block[0]}");
            }

            int step = 1;
            while (sortedBlocks.Count > 1)
            {
                var mergedBlocks = new List<List<string>>();
                progress?.Report($"[Слияние пар блоков]: Шаг {step}");

                for (int i = 0; i < sortedBlocks.Count; i += 2)
                {
                    if (i + 1 < sortedBlocks.Count)
                    {
                        var block1 = sortedBlocks[i];
                        var block2 = sortedBlocks[i + 1];
                        var merged = Merge(block1, block2, secondaryKeyIndex, progress);
                        mergedBlocks.Add(merged);
                    }
                    else
                    {
                        mergedBlocks.Add(sortedBlocks[i]);
                        progress?.Report($"[Блок остается без изменений]\n\n{sortedBlocks[i][0]}");
                    }
                }

                sortedBlocks = mergedBlocks;
                step++;
                Thread.Sleep(delay);
            }

            progress?.Report(
                $"Все строки упорядочены по ключу \"{secondaryKeyName}\".\n\n[Результат]\n\n    • {string.Join("\n\n    • ", sortedBlocks[0])}\n\n[Сортировка завершена]");
            return sortedBlocks[0];
        }

        private List<string> NaturalMergeSort(List<string> data, int secondaryKeyIndex, string secondaryKeyName, IProgress<string> progress, int delay)
        {
            // Нахождение естественных последовательностей
            var runs = FindNaturalRuns(data, secondaryKeyIndex);
            progress?.Report(
                $"[Нахождение естественных последовательностей]\n\nНайдено {runs.Count} последовательностей:");
            foreach (var run in runs)
            {
                progress?.Report($"   • {string.Join(", ", run)}");
            }

            int step = 1;
            while (runs.Count > 1)
            {
                var mergedRuns = new List<List<string>>();
                progress?.Report($"[Слияние последовательностей]: Шаг {step}");

                for (int i = 0; i < runs.Count; i += 2)
                {
                    if (i + 1 < runs.Count)
                    {
                        var run1 = runs[i];
                        var run2 = runs[i + 1];
                        var merged = Merge(run1, run2, secondaryKeyIndex, progress);
                        mergedRuns.Add(merged);
                    }
                    else
                    {
                        mergedRuns.Add(runs[i]);
                        progress?.Report(
                            $"[Последовательность остается без изменений]\n\n    • {string.Join("\n\n    • ", runs[i])}");
                    }
                }

                runs = mergedRuns;
                step++;
                Thread.Sleep(delay);
            }

            progress?.Report(
                $"Все строки упорядочены по ключу \"{secondaryKeyName}\".\n\n[Результат]\n\n    • {string.Join("\n\n    • ", runs.SelectMany(r => r))}\n\n[Сортировка завершена]");
            return runs.SelectMany(r => r).ToList();
        }

        private List<string> MultiwayMergeSort(List<string> data, int secondaryKeyIndex, string secondaryKeyName, IProgress<string> progress, int delay)
        {
            // Разбиение данных на блоки
            var runs = SplitIntoSortedBlocks(data, secondaryKeyIndex);
            progress?.Report($"[Начало многопутевого слияния]\n\nКоличество блоков: {runs.Count}");
            foreach (var run in runs.Select((b, index) => new { b, index }))
            {
                progress?.Report($"    • Блок {run.index + 1}: {run.b[0]}");
            }

            var sortedData = new List<string>();

            var currentIndices = new int[runs.Count];
            for (int i = 0; i < currentIndices.Length; i++)
            {
                currentIndices[i] = 0;
            }

            while (true)
            {
                string minElement = null;
                int minBlockIndex = -1;
                string comparisonDetails = "";

                // Поиск блока с минимальным текущим элементом по вторичному ключу
                for (int i = 0; i < runs.Count; i++)
                {
                    if (currentIndices[i] < runs[i].Count)
                    {
                        string currentElement = runs[i][currentIndices[i]];
                        string currentKey = GetKey(currentElement, secondaryKeyIndex);

                        if (minElement == null)
                        {
                            minElement = currentElement;
                            minBlockIndex = i;
                        }
                        else
                        {
                            string minKey = GetKey(minElement, secondaryKeyIndex);
                            int comparisonResult = string.Compare(currentKey, minKey, StringComparison.Ordinal);

                            if (comparisonResult < 0)
                            {
                                comparisonDetails +=
                                    $"\n\n    • {currentKey} из блока {i + 1} < {minKey} из блока {minBlockIndex + 1}";
                                minElement = currentElement;
                                minBlockIndex = i;
                            }
                            else
                            {
                                comparisonDetails +=
                                    $"\n\n    • {currentKey} из блока {i + 1} >= {minKey} из блока {minBlockIndex + 1}";
                            }
                        }
                    }
                }

                if (minElement == null)
                {
                    break;
                }

                sortedData.Add(minElement);
                progress?.Report(
                    $"\n\n[Сравнение]: Шаг {_stepNumber + 1}\n\nВыбрано \"{minElement}\" из блока {minBlockIndex + 1}.{comparisonDetails}");

                currentIndices[minBlockIndex]++;
                _stepNumber++;
                Thread.Sleep(delay);
            }

            progress?.Report(
                $"Все строки упорядочены по ключу \"{secondaryKeyName}\".\n\n[Результат]\n\n    • {string.Join("\n\n    • ", sortedData)}\n\n[Сортировка завершена]");
            return sortedData;
        }

        private List<string> Merge(List<string> block1, List<string> block2, int secondaryKeyIndex, IProgress<string> progress)
        {
            int i = 0, j = 0;
            var merged = new List<string>();

            progress?.Report($"[Слияние блоков]\n\n   • {block1[0]}\n\n   • {block2[0]}\n\n[Сравнение]");

            while (i < block1.Count && j < block2.Count)
            {
                var key1 = GetKey(block1[i], secondaryKeyIndex);
                var key2 = GetKey(block2[j], secondaryKeyIndex);

                if (string.Compare(key1, key2, StringComparison.Ordinal) < 0)
                {
                    merged.Add(block1[i]);
                    progress?.Report(
                        $"    • {key1} из блока 1 < {key2} из блока 2.\n\n    • Добавляем \"{block1[i]}\" из блока 1.");
                    i++;
                }
                else
                {
                    merged.Add(block2[j]);
                    progress?.Report(
                        $"    • {key1} из блока 1 >= {key2} из блока 2.\n\n    • Добавляем \"{block2[j]}\" из блока 2.");
                    j++;
                }
            }

            // Добавляем оставшиеся элементы из block1, если они есть
            while (i < block1.Count)
            {
                merged.Add(block1[i]);
                progress?.Report($"    • Добавляем оставшийся элемент \"{block1[i]}\" из блока 1.");
                i++;
            }

            // Добавляем оставшиеся элементы из block2, если они есть
            while (j < block2.Count)
            {
                merged.Add(block2[j]);
                progress?.Report($"    • Добавляем оставшийся элемент \"{block2[j]}\" из блока 2.");
                j++;
            }

            progress?.Report($"[Результат слияния]\n\n    • {string.Join("\n\n    • ", merged)}\n");
            return merged;
        }

        private List<List<string>> SplitIntoSortedBlocks(List<string> data, int secondaryKeyIndex)
        {
            // Разбиваем данные на отдельные блоки, каждый элемент — отдельный блок
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

        private string GetKey(string line, int keyIndex)
        {
            var parts = line.Split(',');
            if (keyIndex < 0 || keyIndex >= parts.Length)
                throw new IndexOutOfRangeException("Указанный индекс ключа выходит за пределы таблицы.");

            return parts[keyIndex].Trim();
        }
    }
}