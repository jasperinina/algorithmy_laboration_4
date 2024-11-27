using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class HeapSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int, int)> sortSteps)
        {
            for (int n = array.Length - 1; n > 1; n--)
            {
                BuildMaxHeap(array, n, sortSteps);
                (array[0], array[n]) = (array[n], array[0]);
                sortSteps.Add((0, n, 2));
            }
        }
        public void BuildMaxHeap(int[] array, int last_index, List<(int, int, int)> sortSteps)
        {
            for (int i = last_index; i > 0; i--)
            {
                int parent_id = (i - 1) / 2;
                if (array[i] > array[parent_id])
                {
                    (array[i], array[parent_id]) = (array[parent_id], array[i]);
                    sortSteps.Add((i, parent_id, 3));
                }
                else
                {
                    sortSteps.Add((i, parent_id, 4));
                }
            }
        }
    }
}
