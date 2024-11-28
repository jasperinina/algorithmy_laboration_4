using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class QuickSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int, int, string)> sortSteps)
        {
            QuickSortRecursive(array, 0, array.Length - 1, sortSteps);
        }

        private void QuickSortRecursive(int[] array, int low, int high, List<(int, int, int, string)> sortSteps)
        {
            if (low < high)
            {
                int pi = Partition(array, low, high, sortSteps);
                QuickSortRecursive(array, low, pi - 1, sortSteps);
                QuickSortRecursive(array, pi + 1, high, sortSteps);
            }
        }

        private int Partition(int[] array, int low, int high, List<(int, int, int, string)> sortSteps)
        {
            int pivot = array[high];
            int i = low - 1;
            string bounds = low.ToString() + " " + high.ToString();
            for (int j = low; j < high; j++)
            {
                sortSteps.Add((j, high, 10, bounds));

                if (array[j] < pivot)
                {
                    i++;
                    (array[i], array[j]) = (array[j], array[i]);
                    sortSteps.Add((i, j, 11, bounds));
                }
            }
            (array[i + 1], array[high]) = (array[high], array[i + 1]);
            sortSteps.Add((i + 1, high, 9, bounds));
            return i + 1;
        }
    }
}
