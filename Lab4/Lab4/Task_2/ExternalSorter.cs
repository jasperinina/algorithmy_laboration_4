using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab4.Task_2
{
    public class ExternalSorter
    {
        private readonly FileHandler _fileHandler;
        private int _tempFileCounter = 0;
        private readonly string _workingDirectory;

        public ExternalSorter(FileHandler fileHandler)
        {
            _fileHandler = fileHandler;
            _workingDirectory = AppDomain.CurrentDomain.BaseDirectory; // Текущая рабочая директория
        }

        public void Sort(
            string sortMethod,
            int filterKeyIndex,
            string filterKeyValue,
            int secondaryKeyIndex,
            IProgress<string> progress)
        {
            // Отступы для визуального восприятия
            string indentLevel1 = "  ";
            string indentLevel2 = "    ";
            string indentLevel3 = "      ";

            // Шаг 1: Фильтрация данных
            progress?.Report("[Фильтрация данных]");
            progress?.Report($"{indentLevel1}Выбираем элементы с ключом \"{filterKeyValue}\":");
            var filteredData = FilterData(filterKeyIndex, filterKeyValue, progress, indentLevel2);

            if (filteredData.Count == 0)
            {
                progress?.Report("Нет данных, соответствующих критерию фильтрации.");
                return;
            }

            // Шаг 2: Разбить данные на временные файлы в зависимости от метода сортировки
            progress?.Report("[Разбиение данных]");
            List<string> tempFiles;

            switch (sortMethod)
            {
                case "Прямое слияние":
                    tempFiles = SplitDataForDirectMergeSort(filteredData, secondaryKeyIndex, progress, indentLevel1);
                    break;
                case "Естественное слияние":
                    tempFiles = SplitDataForNaturalMergeSort(filteredData, secondaryKeyIndex, progress, indentLevel1);
                    break;
                case "Многопутевое слияние":
                    tempFiles = SplitDataForMultiwayMergeSort(filteredData, secondaryKeyIndex, progress, indentLevel1);
                    break;
                default:
                    throw new NotSupportedException($"Метод сортировки \"{sortMethod}\" не поддерживается.");
            }

            // Шаг 3: Выполнить сортировку выбранным методом
            progress?.Report($"[Сортировка методом {sortMethod}]");
            string resultFile = sortMethod switch
            {
                "Прямое слияние" => PerformDirectMergeSort(tempFiles, secondaryKeyIndex, progress, indentLevel1),
                "Естественное слияние" => PerformNaturalMergeSort(tempFiles, secondaryKeyIndex, progress, indentLevel1),
                "Многопутевое слияние" => PerformMultiwayMergeSort(tempFiles, secondaryKeyIndex, progress, indentLevel1),
                _ => throw new NotSupportedException($"Метод сортировки \"{sortMethod}\" не поддерживается.")
            };

            // Перемещаем результат в текущую рабочую директорию, перезаписывая файл, если он существует
            var finalResultFile = Path.Combine(_workingDirectory, "SortedResult.txt");
            if (File.Exists(finalResultFile))
            {
                File.Delete(finalResultFile); // Удаляем существующий файл
            }
            File.Move(resultFile, finalResultFile);

            progress?.Report($"[Сортировка завершена. Результат записан в файл: {Path.GetFileName(finalResultFile)}]");

            // Добавляем вывод результата сортировки
            progress?.Report("[Результат сортировки]:");
            var sortedData = File.ReadAllLines(finalResultFile);
            foreach (var line in sortedData)
            {
                progress?.Report($"{indentLevel1}• {line}");
            }
        }

        private List<string> FilterData(int filterKeyIndex, string filterKeyValue, IProgress<string> progress, string indent)
        {
            var data = _fileHandler.ReadFromFile();
            var filteredData = data.Skip(1) // Пропускаем заголовок
                .Where(line => line.Split(',')[filterKeyIndex].Trim() == filterKeyValue)
                .ToList();

            if (filteredData.Count == 0)
            {
                return filteredData;
            }

            progress?.Report($"{indent}Отфильтрованные элементы:");
            foreach (var line in filteredData)
            {
                progress?.Report($"{indent}• {line}");
            }

            // Добавляем заголовок в начало
            filteredData.Insert(0, data[0]);
            return filteredData;
        }

        // Метод для прямого слияния
        private List<string> SplitDataForDirectMergeSort(List<string> data, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            var tempFiles = new List<string>();
            var evenLines = new List<string>();
            var oddLines = new List<string>();

            for (int i = 1; i < data.Count; i++)
            {
                if ((i % 2) == 0)
                {
                    evenLines.Add(data[i]);
                }
                else
                {
                    oddLines.Add(data[i]);
                }
            }

            // Сортировка каждого файла
            evenLines.Sort((line1, line2) =>
            {
                var key1 = GetKey(line1, secondaryKeyIndex);
                var key2 = GetKey(line2, secondaryKeyIndex);
                return string.Compare(key1, key2, StringComparison.Ordinal);
            });

            oddLines.Sort((line1, line2) =>
            {
                var key1 = GetKey(line1, secondaryKeyIndex);
                var key2 = GetKey(line2, secondaryKeyIndex);
                return string.Compare(key1, key2, StringComparison.Ordinal);
            });

            var tempFileEven = GetTempFileName();
            File.WriteAllLines(tempFileEven, evenLines);
            tempFiles.Add(tempFileEven);
            progress?.Report($"{indent}Создан временный файл для четных индексов: {Path.GetFileName(tempFileEven)}");

            var tempFileOdd = GetTempFileName();
            File.WriteAllLines(tempFileOdd, oddLines);
            tempFiles.Add(tempFileOdd);
            progress?.Report($"{indent}Создан временный файл для нечетных индексов: {Path.GetFileName(tempFileOdd)}");

            return tempFiles;
        }

        // Метод для естественного слияния
        private List<string> SplitDataForNaturalMergeSort(List<string> data, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            var tempFiles = new List<string>();
            if (data.Count <= 1) return tempFiles;

            string previousKey = GetKey(data[1], secondaryKeyIndex);
            var currentBlock = new List<string> { data[1] }; // Начинаем с первого элемента

            for (int i = 2; i < data.Count; i++)
            {
                string currentKey = GetKey(data[i], secondaryKeyIndex);

                if (string.Compare(previousKey, currentKey, StringComparison.Ordinal) <= 0)
                {
                    // Последовательность продолжается
                    currentBlock.Add(data[i]);
                }
                else
                {
                    // Последовательность прерывается, сохраняем текущий блок
                    var tempFile = GetTempFileName();
                    File.WriteAllLines(tempFile, currentBlock);
                    tempFiles.Add(tempFile);
                    progress?.Report($"{indent}Создан временный файл для естественного блока: {Path.GetFileName(tempFile)}");
                    currentBlock = new List<string> { data[i] };
                }

                previousKey = currentKey;
            }

            // Сохраняем последний блок
            if (currentBlock.Count > 0)
            {
                var tempFile = GetTempFileName();
                File.WriteAllLines(tempFile, currentBlock);
                tempFiles.Add(tempFile);
                progress?.Report($"{indent}Создан временный файл для естественного блока: {Path.GetFileName(tempFile)}");
            }

            return tempFiles;
        }

        // Метод для многопутевого слияния
        private List<string> SplitDataForMultiwayMergeSort(List<string> data, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            var tempFiles = new List<string>();
            int chunkSize = 1; // Каждый элемент становится отдельным блоком

            for (int i = 1; i < data.Count; i += chunkSize)
            {
                var chunk = data.Skip(i).Take(chunkSize).ToList();

                // Сортируем каждый блок (хотя в данном случае блок из одного элемента)
                chunk.Sort((line1, line2) =>
                {
                    var key1 = GetKey(line1, secondaryKeyIndex);
                    var key2 = GetKey(line2, secondaryKeyIndex);
                    return string.Compare(key1, key2, StringComparison.Ordinal);
                });

                var tempFile = GetTempFileName();
                File.WriteAllLines(tempFile, chunk);
                tempFiles.Add(tempFile);

                progress?.Report($"{indent}Создан временный файл для блока: {Path.GetFileName(tempFile)}");
            }

            return tempFiles;
        }

        // Остальные методы остаются без изменений
        private string PerformDirectMergeSort(List<string> tempFiles, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            int step = 1;
            while (tempFiles.Count > 1)
            {
                progress?.Report($"{indent}[Слияние пар блоков] Шаг {step}");
                var mergedFiles = new List<string>();
                var filesToDelete = new List<string>();

                for (int i = 0; i < tempFiles.Count; i += 2)
                {
                    if (i + 1 < tempFiles.Count)
                    {
                        var mergedFile = MergeFiles(tempFiles[i], tempFiles[i + 1], secondaryKeyIndex, progress, indent + "  ");
                        mergedFiles.Add(mergedFile);

                        progress?.Report($"{indent}Слияние файлов {Path.GetFileName(tempFiles[i])} и {Path.GetFileName(tempFiles[i + 1])} в {Path.GetFileName(mergedFile)}");

                        // Добавляем исходные файлы в список для удаления
                        filesToDelete.Add(tempFiles[i]);
                        filesToDelete.Add(tempFiles[i + 1]);
                    }
                    else
                    {
                        mergedFiles.Add(tempFiles[i]);
                        // Не удаляем tempFiles[i], так как он не был слит
                    }
                }

                // Удаляем только те файлы, которые были использованы в слиянии
                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                }

                tempFiles = mergedFiles;
                step++;
            }

            return tempFiles[0];
        }

        private string PerformNaturalMergeSort(List<string> tempFiles, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            int step = 1;
            while (tempFiles.Count > 1)
            {
                progress?.Report($"{indent}[Слияние блоков] Шаг {step}");
                var mergedFiles = new List<string>();
                var filesToDelete = new List<string>();

                for (int i = 0; i < tempFiles.Count; i += 2)
                {
                    if (i + 1 < tempFiles.Count)
                    {
                        var mergedFile = MergeFiles(tempFiles[i], tempFiles[i + 1], secondaryKeyIndex, progress, indent + "  ");
                        mergedFiles.Add(mergedFile);

                        progress?.Report($"{indent}Слияние файлов {Path.GetFileName(tempFiles[i])} и {Path.GetFileName(tempFiles[i + 1])} в {Path.GetFileName(mergedFile)}");

                        // Добавляем исходные файлы в список для удаления
                        filesToDelete.Add(tempFiles[i]);
                        filesToDelete.Add(tempFiles[i + 1]);
                    }
                    else
                    {
                        mergedFiles.Add(tempFiles[i]);
                        // Не удаляем tempFiles[i], так как он не был слит
                    }
                }

                // Удаляем только те файлы, которые были использованы в слиянии
                foreach (var file in filesToDelete)
                {
                    File.Delete(file);
                }

                tempFiles = mergedFiles;
                step++;
            }

            return tempFiles[0];
        }

        private string PerformMultiwayMergeSort(List<string> tempFiles, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            progress?.Report($"{indent}[Слияние блоков]");
            string resultFile = GetTempFileName();
            using var writer = new StreamWriter(resultFile);

            var readers = tempFiles.Select(file => new StreamReader(file)).ToList();
            var buffer = new List<(string line, StreamReader reader, string fileName)>();

            // Инициализируем буфер
            foreach (var reader in readers)
            {
                if (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    buffer.Add((line, reader, Path.GetFileName(((FileStream)reader.BaseStream).Name)));
                }
            }

            while (buffer.Count > 0)
            {
                // Получаем текущие элементы и их ключи
                var currentItems = buffer.Select(x => new
                {
                    x.line,
                    x.reader,
                    x.fileName,
                    key = GetKey(x.line, secondaryKeyIndex)
                }).ToList();

                // Выводим текущие элементы для сравнения
                progress?.Report($"{indent}Текущие элементы для сравнения:");
                foreach (var item in currentItems)
                {
                    progress?.Report($"{indent}• \"{item.line}\" из {item.fileName}");
                }

                // Находим минимальный элемент
                var minItem = currentItems.OrderBy(x => x.key).First();

                progress?.Report($"{indent}Выбираем минимальный элемент \"{minItem.line}\" из {minItem.fileName} и добавляем в результирующий файл.");
                writer.WriteLine(minItem.line);

                // Обновляем буфер
                var readerToUpdate = buffer.First(x => x.reader == minItem.reader);
                buffer.Remove(readerToUpdate);

                if (!minItem.reader.EndOfStream)
                {
                    string newLine = minItem.reader.ReadLine();
                    buffer.Add((newLine, minItem.reader, minItem.fileName));
                }
            }

            foreach (var reader in readers)
            {
                reader.Dispose();
            }

            // Удаляем временные файлы после использования
            foreach (var file in tempFiles)
            {
                File.Delete(file);
            }

            progress?.Report($"{indent}[Многопутевое слияние завершено]: Результат записан во временный файл {Path.GetFileName(resultFile)}");
            return resultFile;
        }

        private string MergeFiles(string file1, string file2, int secondaryKeyIndex, IProgress<string> progress, string indent)
        {
            var tempFile = GetTempFileName();
            using var reader1 = new StreamReader(file1);
            using var reader2 = new StreamReader(file2);
            using var writer = new StreamWriter(tempFile);

            string line1 = reader1.ReadLine();
            string line2 = reader2.ReadLine();

            progress?.Report($"{indent}[Сравнение элементов из файлов {Path.GetFileName(file1)} и {Path.GetFileName(file2)}]");
            while (line1 != null && line2 != null)
            {
                var key1 = GetKey(line1, secondaryKeyIndex);
                var key2 = GetKey(line2, secondaryKeyIndex);

                progress?.Report($"{indent}Сравниваем \"{key1}\" из {Path.GetFileName(file1)} и \"{key2}\" из {Path.GetFileName(file2)}");

                if (string.Compare(key1, key2, StringComparison.Ordinal) <= 0)
                {
                    progress?.Report($"{indent}Добавляем \"{line1}\" из {Path.GetFileName(file1)} в результирующий файл, потому что \"{key1}\" ≤ \"{key2}\".");
                    writer.WriteLine(line1);
                    line1 = reader1.ReadLine();
                }
                else
                {
                    progress?.Report($"{indent}Добавляем \"{line2}\" из {Path.GetFileName(file2)} в результирующий файл, потому что \"{key1}\" > \"{key2}\".");
                    writer.WriteLine(line2);
                    line2 = reader2.ReadLine();
                }
            }

            while (line1 != null)
            {
                progress?.Report($"{indent}Добавляем оставшийся элемент \"{line1}\" из {Path.GetFileName(file1)} в результирующий файл.");
                writer.WriteLine(line1);
                line1 = reader1.ReadLine();
            }

            while (line2 != null)
            {
                progress?.Report($"{indent}Добавляем оставшийся элемент \"{line2}\" из {Path.GetFileName(file2)} в результирующий файл.");
                writer.WriteLine(line2);
                line2 = reader2.ReadLine();
            }

            return tempFile;
        }

        private string GetKey(string line, int keyIndex)
        {
            var parts = line.Split(',');
            return parts[keyIndex].Trim();
        }

        private string GetTempFileName()
        {
            return Path.Combine(_workingDirectory, $"temp_sort_file_{_tempFileCounter++}.txt");
        }
    }
}
