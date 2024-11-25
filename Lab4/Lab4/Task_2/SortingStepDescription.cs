using System.Text;

namespace Lab4.Task_2;

public class SortingStepDescription
{
    private int _stepCount; // Счетчик шагов

    public SortingStepDescription()
    {
        _stepCount = 0;
    }
    /*
    public string GetStepDescription(
        int stepNumber,
        string element1,
        string element2,
        string action)
    {
        var description = new StringBuilder(); description.AppendLine($"Шаг {stepNumber}:");

       description.AppendLine($"Сравниваем элементы: {element1} и {element2}.");
        description.AppendLine($"Действие: {action}.");

        return description.ToString();
    }
    */
}
