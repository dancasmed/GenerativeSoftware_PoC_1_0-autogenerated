using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class WaterIntakeTracker : IGeneratedModule
{
    public string Name { get; set; } = "Water Intake Tracker";
    
    private string _dataFilePath;
    private const int RecommendedDailyIntake = 2000; // in milliliters
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Water Intake Tracker module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "water_intake.json");
        
        var today = DateTime.Today;
        var intakeData = LoadIntakeData();
        
        if (intakeData.Date != today)
        {
            intakeData = new DailyIntake { Date = today, IntakeMilliliters = 0 };
        }
        
        Console.WriteLine("Current water intake: " + intakeData.IntakeMilliliters + " ml");
        Console.WriteLine("Recommended daily intake: " + RecommendedDailyIntake + " ml");
        
        if (intakeData.IntakeMilliliters < RecommendedDailyIntake)
        {
            int remaining = RecommendedDailyIntake - intakeData.IntakeMilliliters;
            Console.WriteLine("You need to drink " + remaining + " ml more today.");
            
            Console.Write("How much water did you drink (ml)? ");
            if (int.TryParse(Console.ReadLine(), out int amount) && amount > 0)
            {
                intakeData.IntakeMilliliters += amount;
                SaveIntakeData(intakeData);
                Console.WriteLine("Updated intake: " + intakeData.IntakeMilliliters + " ml");
            }
            else
            {
                Console.WriteLine("Invalid amount. No changes made.");
            }
        }
        else
        {
            Console.WriteLine("Congratulations! You've met your daily water intake goal.");
        }
        
        return true;
    }
    
    private DailyIntake LoadIntakeData()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                return JsonSerializer.Deserialize<DailyIntake>(json) ?? new DailyIntake();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading intake data: " + ex.Message);
        }
        
        return new DailyIntake();
    }
    
    private void SaveIntakeData(DailyIntake data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving intake data: " + ex.Message);
        }
    }
    
    private class DailyIntake
    {
        public DateTime Date { get; set; } = DateTime.Today;
        public int IntakeMilliliters { get; set; } = 0;
    }
}