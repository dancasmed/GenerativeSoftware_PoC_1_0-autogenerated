using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;

public class DaysBetweenDatesModule : IGeneratedModule
{
    public string Name { get; set; } = "Days Between Dates Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Days Between Dates Module is running...");
        Console.WriteLine("This module calculates the number of days between two dates.");

        DateTime startDate;
        DateTime endDate;

        Console.Write("Enter the start date (yyyy-MM-dd): ");
        string startDateInput = Console.ReadLine();

        Console.Write("Enter the end date (yyyy-MM-dd): ");
        string endDateInput = Console.ReadLine();

        if (!DateTime.TryParse(startDateInput, out startDate) || !DateTime.TryParse(endDateInput, out endDate))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-MM-dd format.");
            return false;
        }

        TimeSpan difference = endDate - startDate;
        int daysDifference = difference.Days;

        Console.WriteLine("The number of days between the two dates is: " + daysDifference);

        string resultFilePath = Path.Combine(dataFolder, "days_between_dates_result.json");
        string jsonResult = $"{{\"startDate\": \"{startDate:yyyy-MM-dd}\", \"endDate\": \"{endDate:yyyy-MM-dd}\", \"daysDifference\": {daysDifference}}}";

        try
        {
            File.WriteAllText(resultFilePath, jsonResult);
            Console.WriteLine("Result saved to: " + resultFilePath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving result: " + ex.Message);
            return false;
        }
    }
}