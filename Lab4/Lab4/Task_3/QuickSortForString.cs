using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_3
{
    class QuickSortForString
    {
        public string[] _text;

        public QuickSortForString(string text)
        {
            _text = text.ToLower().Split(" ");
            Sort(_text, 0, _text.Length - 1);
        }

        public void Sort(string[] arr, int low, int high)
        {
            if (low < high)
            {
                // Разделить массив и получить индекс разделения
                int pivotIndex = Partition(arr, low, high);

                // Рекурсивно сортируем элементы до и после разделения
                Sort(arr, low, pivotIndex - 1);
                Sort(arr, pivotIndex + 1, high);
            }
        }

        // Метод, который выбирает опорный элемент и размещает элементы вокруг него
        private static int Partition(string[] arr, int low, int high)
        {
            // Опорный элемент
            string pivot = arr[high];
            int i = (low - 1); // Индекс меньшего элемента

            for (int j = low; j < high; j++)
            {
                // Если текущий элемент меньше или равен опорному
                if (String.Compare(arr[j], pivot) <= 0)
                {
                    i++;

                    // Меняем местами arr[i] и arr[j]
                    string temp = arr[i];
                    arr[i] = arr[j];
                    arr[j] = temp;
                }
            }

            // Меняем местами arr[i + 1] и arr[high] (или опорный элемент)
            string temp1 = arr[i + 1];
            arr[i + 1] = arr[high];
            arr[high] = temp1;

            return i + 1;
        }

        public void print()
        {
            for (int i = 0; i < _text.Length; i++)
            {
                Console.Write(_text[i] + " ");
            }
        }
    }
}
