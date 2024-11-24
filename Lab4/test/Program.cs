
class Program
{
    static void Main(string[] args)
    {
        string taskFolder = "Task_2"; // Папка задания
        var fileHandler = new FileHandlerTask_2(taskFolder);
        var logHandler = new LogHandlerTask_2();
        var sorter = new ExternalSorterTask_2(fileHandler, logHandler);

        string inputFileName = "input.txt";  // Входной файл в папке Files
        string outputFileName = "output.txt"; // Выходной файл в папке Files
        string key = "Population"; // Ключ для сортировки

        try
        {
            // Сортировка методом прямого слияния
            sorter.Sort(inputFileName, outputFileName, key, SortMethodTask_2.DirectMerge);

            // Печать логов операций
            Console.WriteLine("Логи сортировки:");
            logHandler.PrintLogs();

            Console.WriteLine($"Сортировка завершена. Результаты сохранены в {fileHandler.GetFilePath(outputFileName)}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}