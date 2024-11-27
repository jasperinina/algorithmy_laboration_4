using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class QuickSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int, int)> sortSteps)
        {
            QuickSortRecursive(array, 0, array.Length - 1, sortSteps);
        }

        private void QuickSortRecursive(int[] array, int low, int high, List<(int, int, int)> sortSteps)
        {
            if (low < high)
            {
                int pi = Partition(array, low, high, sortSteps);
                QuickSortRecursive(array, low, pi - 1, sortSteps);
                QuickSortRecursive(array, pi + 1, high, sortSteps);
            }
        }

        private int Partition(int[] array, int low, int high, List<(int, int, int)> sortSteps)
        {
            int pivot = array[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                // Добавляем действие сравнения в sortSteps
                sortSteps.Add((j, high, 0));

                if (array[j] < pivot)
                {
                    i++;
                    (array[i], array[j]) = (array[j], array[i]);
                    // Добавляем действие перестановки в sortSteps
                    sortSteps.Add((i, j, 1));
                }
            }

            (array[i + 1], array[high]) = (array[high], array[i + 1]);
            // Добавляем действие перестановки в sortSteps
            sortSteps.Add((i + 1, high, 1));
            return i + 1;
        }
    }
}
