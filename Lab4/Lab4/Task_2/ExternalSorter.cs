using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4.Task_2;

public class ExternalSorter
{
    private readonly FileHandler _fileHandler;

    public ExternalSorter(FileHandler fileHandler)
    {
        _fileHandler = fileHandler;
    }

    public void Sort(string sortMethod, int primaryKeyIndex, int secondaryKeyIndex, Action<List<string>> visualizeStep, int delay)
    {
        var lines = _fileHandler.ReadFromFile();
        var headers = lines[0]; // Первая строка — заголовок
        var data = lines.Skip(1).ToList(); // Остальные строки — данные

        var stepDescription = new SortingStepDescription();

        List<string> sortedData = sortMethod switch
        {
            "Прямое слияние" => DirectMergeSort(data, primaryKeyIndex, secondaryKeyIndex, visualizeStep, stepDescription, delay),
            "Естественное слияние" => NaturalMergeSort(data, primaryKeyIndex, secondaryKeyIndex, visualizeStep, stepDescription, delay),
            "Многопутевое слияние" => MultiwayMergeSort(data, primaryKeyIndex, secondaryKeyIndex, visualizeStep, stepDescription, delay),
            _ => throw new NotImplementedException("Выбранный метод сортировки не поддерживается.")
        };

        // Записываем результат в файл
        _fileHandler.WriteToFile(new List<string> { headers }.Concat(sortedData).ToList());
    }

    private List<string> DirectMergeSort(List<string> data, int primaryKeyIndex, int secondaryKeyIndex,
        Action<List<string>> visualizeStep, SortingStepDescription stepDescription, int delay)
    {
        var sortedBlocks = SplitIntoSortedBlocks(data, primaryKeyIndex, secondaryKeyIndex);

        while (sortedBlocks.Count > 1)
        {
            var mergedBlocks = new List<List<string>>();

            for (int i = 0; i < sortedBlocks.Count; i += 2)
            {
                if (i + 1 < sortedBlocks.Count)
                {
                    var block1 = sortedBlocks[i];
                    var block2 = sortedBlocks[i + 1];
                    var merged = Merge(block1, block2, primaryKeyIndex, secondaryKeyIndex, stepDescription);

                    mergedBlocks.Add(merged);
                }
                else
                {
                    mergedBlocks.Add(sortedBlocks[i]);
                }
            }

            sortedBlocks = mergedBlocks;

            // Визуализация текущего шага
            visualizeStep(sortedBlocks.SelectMany(b => b).ToList());
            System.Threading.Thread.Sleep(delay);
        }

        return sortedBlocks[0];
    }

    private List<string> NaturalMergeSort(List<string> data, int primaryKeyIndex, int secondaryKeyIndex,
        Action<List<string>> visualizeStep, SortingStepDescription stepDescription, int delay)
    {
        var runs = FindNaturalRuns(data, primaryKeyIndex, secondaryKeyIndex);

        while (runs.Count > 1)
        {
            var mergedRuns = new List<List<string>>();

            for (int i = 0; i < runs.Count; i += 2)
            {
                if (i + 1 < runs.Count)
                {
                    var run1 = runs[i];
                    var run2 = runs[i + 1];
                    var merged = Merge(run1, run2, primaryKeyIndex, secondaryKeyIndex, stepDescription);

                    mergedRuns.Add(merged);
                }
                else
                {
                    mergedRuns.Add(runs[i]);
                }
            }

            runs = mergedRuns;

            // Визуализация текущего шага
            visualizeStep(runs.SelectMany(r => r).ToList());
            System.Threading.Thread.Sleep(delay);
        }

        return runs[0];
    }

    private List<string> MultiwayMergeSort(List<string> data, int primaryKeyIndex, int secondaryKeyIndex,
        Action<List<string>> visualizeStep, SortingStepDescription stepDescription, int delay)
    {
        var runs = SplitIntoSortedBlocks(data, primaryKeyIndex, secondaryKeyIndex);

        while (runs.Count > 1)
        {
            var mergedRuns = MergeMultiway(runs, primaryKeyIndex, secondaryKeyIndex, stepDescription);

            // Визуализация текущего шага
            visualizeStep(mergedRuns);
            System.Threading.Thread.Sleep(delay);

            runs = SplitIntoSortedBlocks(mergedRuns, primaryKeyIndex, secondaryKeyIndex);
        }

        return runs.SelectMany(r => r).ToList();
    }

    private List<List<string>> SplitIntoSortedBlocks(List<string> data, int primaryKeyIndex, int secondaryKeyIndex)
    {
        return data.Select(d => new List<string> { d })
            .OrderBy(b => GetKey(b[0], primaryKeyIndex))
            .ThenBy(b => GetKey(b[0], secondaryKeyIndex))
            .ToList();
    }

    private List<List<string>> FindNaturalRuns(List<string> data, int primaryKeyIndex, int secondaryKeyIndex)
    {
        var runs = new List<List<string>>();
        var currentRun = new List<string> { data[0] };

        for (int i = 1; i < data.Count; i++)
        {
            var currentPrimaryKey = GetKey(data[i], primaryKeyIndex);
            var previousPrimaryKey = GetKey(data[i - 1], primaryKeyIndex);

            var currentSecondaryKey = GetKey(data[i], secondaryKeyIndex);
            var previousSecondaryKey = GetKey(data[i - 1], secondaryKeyIndex);

            if (string.Compare(currentPrimaryKey, previousPrimaryKey) > 0 ||
                (currentPrimaryKey == previousPrimaryKey && string.Compare(currentSecondaryKey, previousSecondaryKey) >= 0))
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

    private List<string> Merge(List<string> block1, List<string> block2, int primaryKeyIndex, int secondaryKeyIndex,
        SortingStepDescription stepDescription)
    {
        int i = 0, j = 0;
        var merged = new List<string>();

        while (i < block1.Count && j < block2.Count)
        {
            var primaryKey1 = GetKey(block1[i], primaryKeyIndex);
            var primaryKey2 = GetKey(block2[j], primaryKeyIndex);

            var secondaryKey1 = GetKey(block1[i], secondaryKeyIndex);
            var secondaryKey2 = GetKey(block2[j], secondaryKeyIndex);

            if (string.Compare(primaryKey1, primaryKey2) < 0 ||
                (primaryKey1 == primaryKey2 && string.Compare(secondaryKey1, secondaryKey2) <= 0))
            {
                merged.Add(block1[i]);
                i++;
            }
            else
            {
                merged.Add(block2[j]);
                j++;
            }

            var action = i < block1.Count && j < block2.Count && block1[i] != block2[j]
                ? "Поменять местами"
                : "Оставить порядок";

            Console.WriteLine(stepDescription.GetStepDescription(
                merged,
                block1[i],
                block2[j],
                primaryKey1,
                primaryKey2,
                secondaryKey1,
                secondaryKey2,
                action
            ));
        }

        merged.AddRange(block1.Skip(i));
        merged.AddRange(block2.Skip(j));

        return merged;
    }

    private List<string> MergeMultiway(List<List<string>> blocks, int primaryKeyIndex, int secondaryKeyIndex,
        SortingStepDescription stepDescription)
    {
        var sortedData = new List<string>();

        foreach (var block in blocks)
        {
            sortedData.AddRange(block.OrderBy(item => GetKey(item, primaryKeyIndex))
                .ThenBy(item => GetKey(item, secondaryKeyIndex)));
        }

        return sortedData;
    }

    private string GetKey(string line, int keyIndex)
    {
        var parts = line.Split(',');
        if (keyIndex < 0 || keyIndex >= parts.Length)
            throw new IndexOutOfRangeException("Указанный индекс ключа выходит за пределы таблицы.");

        return parts[keyIndex].Trim();
    }
}