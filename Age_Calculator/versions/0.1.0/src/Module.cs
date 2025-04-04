using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class AgeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Age Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Age Calculator Module is running...");
        
        string configFile = Path.Combine(dataFolder, "config.json");
        DateTime birthDate;
        
        if (File.Exists(configFile))
        {
            try
            {
                string json = File.ReadAllText(configFile);
                var config = JsonSerializer.Deserialize<Config>(json);
                birthDate = config.BirthDate;
                Console.WriteLine("Birthdate loaded from config file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading config file: " + ex.Message);
                return false;
            }
        }
        else
        {
            Console.WriteLine("Config file not found. Please enter your birthdate (YYYY-MM-DD):");
            string input = Console.ReadLine();
            
            if (!DateTime.TryParse(input, out birthDate))
            {
                Console.WriteLine("Invalid date format. Please use YYYY-MM-DD.");
                return false;
            }
            
            try
            {
                var config = new Config { BirthDate = birthDate };
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(configFile, json);
                Console.WriteLine("Birthdate saved to config file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving config file: " + ex.Message);
                return false;
            }
        }
        
        DateTime currentDate = DateTime.Now;
        TimeSpan age = currentDate - birthDate;
        
        int years = currentDate.Year - birthDate.Year;
        if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
        {
            years--;
        }
        
        int months = currentDate.Month - birthDate.Month;
        if (currentDate.Day < birthDate.Day)
        {
            months--;
        }
        if (months < 0)
        {
            months += 12;
        }
        
        int days = currentDate.Day - birthDate.Day;
        if (days < 0)
        {
            days += DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
        }
        
        Console.WriteLine("Your age is: " + years + " years, " + months + " months, and " + days + " days.");
        return true;
    }
    
    private class Config
    {
        public DateTime BirthDate { get; set; }
    }
}