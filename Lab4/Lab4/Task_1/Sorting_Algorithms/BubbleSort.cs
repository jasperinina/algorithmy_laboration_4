﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1.Sorting_Algorithms
{
    public class BubbleSort : SortingAlgorithm
    {
        public override void Sort(int[] array, List<(int, int)> sortSteps)
        {
            int n = array.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                        sortSteps.Add((j, j + 1));
                    }
                }
            }
        }
    }
}
