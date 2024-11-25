using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class HeapSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int, bool)> sortSteps)
        {
            int n = array.Length;

            // Build max heap
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                Heapify(array, n, i, sortSteps);
            }

            // Extract elements from heap
            for (int i = n - 1; i > 0; i--)
            {
                (array[0], array[i]) = (array[i], array[0]);
                sortSteps.Add((0, i, true));
                Heapify(array, i, 0, sortSteps);
            }
        }

        private void Heapify(int[] array, int n, int i, List<(int, int, bool)> sortSteps)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && array[left] > array[largest])
            {
                largest = left;
            }

            if (right < n && array[right] > array[largest])
            {
                largest = right;
            }

            if (largest != i)
            {
                (array[i], array[largest]) = (array[largest], array[i]);
                sortSteps.Add((largest, i, true));
                Heapify(array, n, largest, sortSteps);
            }
            else
            {
                sortSteps.Add((largest, i, false));
            }
        }
    }
}
