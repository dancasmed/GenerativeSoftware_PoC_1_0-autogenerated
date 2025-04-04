using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class WeatherForecastModule : IGeneratedModule
{
    public string Name { get; set; } = "Weather Forecast Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Weather Forecast Module is running...");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var forecast = GenerateRandomForecast();
            string json = JsonSerializer.Serialize(forecast);
            string filePath = Path.Combine(dataFolder, "weather_forecast.json");
            File.WriteAllText(filePath, json);

            Console.WriteLine("Weather forecast generated and saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while generating the weather forecast: " + ex.Message);
            return false;
        }
    }

    private WeatherForecast GenerateRandomForecast()
    {
        var random = new Random();
        string[] conditions = { "Sunny", "Cloudy", "Rainy", "Snowy", "Windy" };
        string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        
        var forecast = new WeatherForecast
        {
            Location = "Random City",
            GeneratedDate = DateTime.Now,
            DailyForecasts = new DailyForecast[7]
        };

        for (int i = 0; i < 7; i++)
        {
            forecast.DailyForecasts[i] = new DailyForecast
            {
                Day = days[i],
                TemperatureC = random.Next(-10, 35),
                Condition = conditions[random.Next(conditions.Length)],
                Humidity = random.Next(30, 100),
                WindSpeed = random.Next(0, 30)
            };
        }

        return forecast;
    }
}

public class WeatherForecast
{
    public string Location { get; set; }
    public DateTime GeneratedDate { get; set; }
    public DailyForecast[] DailyForecasts { get; set; }
}

public class DailyForecast
{
    public string Day { get; set; }
    public int TemperatureC { get; set; }
    public string Condition { get; set; }
    public int Humidity { get; set; }
    public int WindSpeed { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}