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
                        sortSteps.Add((j, minIndex, 5));
                        minIndex = j;
                    }
                    else
                    {
                        sortSteps.Add((j, minIndex, 6));
                    }
                }

                if (minIndex != i)
                {
                    (array[i], array[minIndex]) = (array[minIndex], array[i]);
                    sortSteps.Add((i, minIndex, 7));
                }
                else
                {
                    sortSteps.Add((i, minIndex, 8));
                }
            }
        }
    }
}
