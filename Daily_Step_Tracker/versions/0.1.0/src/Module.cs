using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class StepTrackerModule
{
    public string Name { get; set; }
    
    public StepTrackerModule()
    {
        Name = "Daily Step Tracker";
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Daily Step Tracker module is running...");
        
        string filePath = Path.Combine(dataFolder, "step_data.json");
        
        StepData data = LoadStepData(filePath);
        
        Console.WriteLine("Enter today's step count:");
        string input = Console.ReadLine();
        
        if (int.TryParse(input, out int steps))
        {
            DateTime today = DateTime.Today;
            
            if (data.Date == today)
            {
                Console.WriteLine("Updating today's step count...");
                data.Steps = steps;
            }
            else
            {
                Console.WriteLine("Recording new step count for today...");
                data = new StepData { Date = today, Steps = steps };
            }
            
            double distance = CalculateDistance(steps);
            Console.WriteLine(string.Format("You've walked {0} steps today, approximately {1:F2} kilometers.", steps, distance));
            
            SaveStepData(filePath, data);
            return true;
        }
        else
        {
            Console.WriteLine("Invalid input. Please enter a valid number of steps.");
            return false;
        }
    }
    
    private StepData LoadStepData(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<StepData>(json) ?? new StepData();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Error loading step data: {0}", ex.Message));
        }
        
        return new StepData();
    }
    
    private void SaveStepData(string filePath, StepData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(string.Format("Error saving step data: {0}", ex.Message));
        }
    }
    
    private double CalculateDistance(int steps)
    {
        // Average step length in meters (approximately 0.762m or 2.5ft)
        const double stepLength = 0.762;
        // Convert meters to kilometers
        return (steps * stepLength) / 1000;
    }
}

public class StepData
{
    public DateTime Date { get; set; } = DateTime.Today;
    public int Steps { get; set; } = 0;
}