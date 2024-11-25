using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_1
{
    public class DataGenerator
    {
        private Random rnd = new Random();
        public int[] ArrayGenerate(int arrayLength, int maxRandomValue)
        {
            int[] array = new int[arrayLength];

            for (int i = 0; i < arrayLength; i++)
            {
                array[i] = rnd.Next(1, maxRandomValue);
            }

            return array;
        }
    }
}
