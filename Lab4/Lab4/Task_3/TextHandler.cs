using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab4.Task_3
{
    class TextHandler
    {
        public string Text;
        private static Random _random = new Random();

        // Принимает длину текста (количество слов)
        public TextHandler(int length)
        {
            Text = GenerateText(length);
        }

        // Для считывания текста с файла, аргумент - путь к файлу
        public TextHandler(string path)
        {
            string strings = string.Empty;
            StreamReader reader = new StreamReader(path, Encoding.ASCII);
            strings = reader.ReadToEnd();
            ParseText(strings);
        }

        // Парсинг текста (удаляет все символы, если это не пробел или не буква)
        private void ParseText(string text)
        {
            string result = new string(text.Where(t => char.IsAsciiLetter(t) || t == ' ').ToArray());
            Text = result.ToLower();
        }

        // генерирует текст, принимая его длину
        public static string GenerateText(int countOfWords)
        {
            string alf = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM"; // набор букв, из которого будут состоять слова текста
            StringBuilder result = new StringBuilder();
            for (int i = 1; i <= countOfWords; i++)
            {
                int lengthWords = _random.Next(1, 15);
                for (int j = 1; j <= lengthWords; j++)
                {
                    int indexLetter = _random.Next(0, alf.Length - 1);
                    result.AppendFormat(Convert.ToString(alf[indexLetter]));
                }
                if (i != countOfWords)
                {
                    result.AppendFormat(" ");
                }
            }
            return Convert.ToString(result);
        }

        // Считает сколько раз слово повторялось в тексте
        public static string CountWord(string[] wordsArr)
        {
            Dictionary<string, int> words = new Dictionary<string, int>();
            for (int i = 0; i < wordsArr.Length; i++)
            {
                if (words.ContainsKey(wordsArr[i]))
                {
                    words[wordsArr[i]]++;
                }
                else
                {
                    words.Add(wordsArr[i], 1);
                }
            }

            string result = string.Empty;
            foreach (var word in words)
            {
                result += string.Format("{0}: {1} \n", word.Key, word.Value);
            }

            return result[..(result.Length - 2)];
        }
    }
}
