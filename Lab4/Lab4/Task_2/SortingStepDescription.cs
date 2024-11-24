using System.Text;

namespace Lab4.Task_2;

public class SortingStepDescription
{
    private int _stepCount; // Счетчик шагов

    public SortingStepDescription()
    {
        _stepCount = 0;
    }
    
    public string GetStepDescription(
        List<string> currentData, 
        string element1, 
        string element2, 
        string primaryKeyValue1, 
        string primaryKeyValue2, 
        string secondaryKeyValue1, 
        string secondaryKeyValue2, 
        string action)
    {
        _stepCount++;

        var description = new StringBuilder();
        description.AppendLine($"Шаг {_stepCount}:");

        description.AppendLine($"Сравниваем: \"{element1}\" ({primaryKeyValue1}, {secondaryKeyValue1}) и \"{element2}\" ({primaryKeyValue2}, {secondaryKeyValue2}).");
        description.AppendLine($"Действие: {action}.");

        description.AppendLine("Текущий порядок данных:");
        foreach (var line in currentData)
        {
            description.AppendLine(line);
        }

        description.AppendLine(new string('-', 50)); // Разделитель
        return description.ToString();
    }
}
