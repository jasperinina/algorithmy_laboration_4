using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class SelectionSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int, int)> sortSteps)
        {
            int n = array.Length;

            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;

                // Находим минимальный элемент в неотсортированной части массива
                for (int j = i + 1; j < n; j++)
                {
                    if (array[j] < array[minIndex])
                    {
                        minIndex = j;
                    }
                }

                bool shouldSwap = minIndex != i;
                if (shouldSwap)
                {
                    (array[i], array[minIndex]) = (array[minIndex], array[i]);
                }
                sortSteps.Add((i, minIndex, shouldSwap ? 1 : 0));
            }
        }
    }
}
