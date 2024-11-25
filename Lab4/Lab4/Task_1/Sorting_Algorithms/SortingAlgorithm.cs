using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public abstract class SortingAlgorithm
    {
        // Базовый метод для сортировки массивов
        public abstract void Sort(int[] array, List<(int, int, bool)> sortSteps);
    }
}
