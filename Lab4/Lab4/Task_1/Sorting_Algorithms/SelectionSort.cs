using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class SelectionSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int)> sortSteps)
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

                // Меняем местами найденный минимальный элемент с первым элементом в неотсортированной части
                if (minIndex != i)
                {
                    int temp = array[i];
                    array[i] = array[minIndex];
                    array[minIndex] = temp;
                    sortSteps.Add((i, minIndex));
                }
            }
        }
    }
}
