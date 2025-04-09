using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WeatherForecastModule : IGeneratedModule
{
    public string Name { get; set; } = "Basic Weather Forecast Module";

    private List<WeatherForecast> forecasts;
    private string dataFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Basic Weather Forecast Module...");
        
        dataFilePath = Path.Combine(dataFolder, "weather_forecasts.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        LoadForecasts();
        
        GenerateSampleForecasts();
        
        DisplayForecasts();
        
        SaveForecasts();
        
        Console.WriteLine("Weather forecast simulation completed successfully.");
        
        return true;
    }

    private void LoadForecasts()
    {
        try
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(json);
                Console.WriteLine("Loaded existing weather forecasts.");
            }
            else
            {
                forecasts = new List<WeatherForecast>();
                Console.WriteLine("No existing forecasts found. Starting with empty list.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading forecasts: " + ex.Message);
            forecasts = new List<WeatherForecast>();
        }
    }

    private void GenerateSampleForecasts()
    {
        string[] conditions = { "Sunny", "Cloudy", "Rainy", "Snowy", "Windy" };
        Random random = new Random();
        
        for (int i = 0; i < 5; i++)
        {
            forecasts.Add(new WeatherForecast
            {
                Date = DateTime.Now.AddDays(i),
                TemperatureC = random.Next(-10, 35),
                Condition = conditions[random.Next(conditions.Length)],
                Humidity = random.Next(30, 100)
            });
        }
        
        Console.WriteLine("Generated 5 new weather forecasts.");
    }

    private void DisplayForecasts()
    {
        Console.WriteLine("\nCurrent Weather Forecasts:");
        Console.WriteLine("------------------------");
        
        foreach (var forecast in forecasts)
        {
            Console.WriteLine("Date: " + forecast.Date.ToShortDateString());
            Console.WriteLine("Temperature: " + forecast.TemperatureC + "°C (" + (32 + (int)(forecast.TemperatureC / 0.5556)) + "°F)");
            Console.WriteLine("Condition: " + forecast.Condition);
            Console.WriteLine("Humidity: " + forecast.Humidity + "%");
            Console.WriteLine();
        }
    }

    private void SaveForecasts()
    {
        try
        {
            string json = JsonSerializer.Serialize(forecasts);
            File.WriteAllText(dataFilePath, json);
            Console.WriteLine("Weather forecasts saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving forecasts: " + ex.Message);
        }
    }
}

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureC { get; set; }
    public string Condition { get; set; }
    public int Humidity { get; set; }
}