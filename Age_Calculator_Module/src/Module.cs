using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class AgeCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Age Calculator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Age Calculator Module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "config.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Configuration file not found. Creating default configuration.");
                var defaultConfig = new { BirthDate = new DateTime(1990, 1, 1) };
                string json = JsonSerializer.Serialize(defaultConfig);
                File.WriteAllText(configPath, json);
                Console.WriteLine("Please update the birthdate in the config file and restart the module.");
                return false;
            }

            string jsonContent = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<Config>(jsonContent);
            
            if (config == null || config.BirthDate == default(DateTime))
            {
                Console.WriteLine("Invalid birthdate in configuration file.");
                return false;
            }

            DateTime birthDate = config.BirthDate;
            DateTime currentDate = DateTime.Now;

            if (birthDate > currentDate)
            {
                Console.WriteLine("Birthdate cannot be in the future.");
                return false;
            }

            int years = currentDate.Year - birthDate.Year;
            int months = currentDate.Month - birthDate.Month;
            int days = currentDate.Day - birthDate.Day;

            if (days < 0)
            {
                months--;
                days += DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            }

            if (months < 0)
            {
                years--;
                months += 12;
            }

            Console.WriteLine("Calculated Age:");
            Console.WriteLine("Years: " + years);
            Console.WriteLine("Months: " + months);
            Console.WriteLine("Days: " + days);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private class Config
    {
        public DateTime BirthDate { get; set; }
    }
}