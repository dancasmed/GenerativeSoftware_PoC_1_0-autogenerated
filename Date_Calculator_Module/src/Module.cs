using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class DateCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Date Calculator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Date Calculator Module is running.");
        Console.WriteLine("This module calculates the number of days between two dates.");

        try
        {
            string configPath = Path.Combine(dataFolder, "date_config.json");
            DateConfig config;

            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<DateConfig>(json);
            }
            else
            {
                config = new DateConfig
                {
                    StartDate = new DateTime(2023, 1, 1),
                    EndDate = DateTime.Now
                };

                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
            }

            TimeSpan difference = config.EndDate - config.StartDate;
            int daysDifference = (int)difference.TotalDays;

            Console.WriteLine("Start Date: " + config.StartDate.ToShortDateString());
            Console.WriteLine("End Date: " + config.EndDate.ToShortDateString());
            Console.WriteLine("Number of days between the dates: " + daysDifference);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private class DateConfig
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}