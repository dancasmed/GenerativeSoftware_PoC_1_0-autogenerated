using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class AirQualityTracker : IGeneratedModule
{
    public string Name { get; set; } = "Air Quality Tracker";
    
    private const string DataFileName = "air_quality_data.json";
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Air Quality Tracker module is running.");
        
        try
        {
            string filePath = Path.Combine(dataFolder, DataFileName);
            List<AirQualityData> data = LoadData(filePath);
            
            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.WriteLine("\nAir Quality Tracker Menu:");
                Console.WriteLine("1. Add new air quality reading");
                Console.WriteLine("2. View all readings");
                Console.WriteLine("3. Analyze data");
                Console.WriteLine("4. Save and exit");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddNewReading(data);
                        break;
                    case "2":
                        DisplayAllReadings(data);
                        break;
                    case "3":
                        AnalyzeData(data);
                        break;
                    case "4":
                        SaveData(filePath, data);
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
    
    private List<AirQualityData> LoadData(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<AirQualityData>>(json);
        }
        return new List<AirQualityData>();
    }
    
    private void SaveData(string filePath, List<AirQualityData> data)
    {
        string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        Console.WriteLine("Data saved successfully.");
    }
    
    private void AddNewReading(List<AirQualityData> data)
    {
        Console.Write("Enter location: ");
        string location = Console.ReadLine();
        
        Console.Write("Enter PM2.5 level: ");
        double pm25 = double.Parse(Console.ReadLine());
        
        Console.Write("Enter PM10 level: ");
        double pm10 = double.Parse(Console.ReadLine());
        
        Console.Write("Enter ozone level: ");
        double ozone = double.Parse(Console.ReadLine());
        
        Console.Write("Enter nitrogen dioxide level: ");
        double no2 = double.Parse(Console.ReadLine());
        
        Console.Write("Enter carbon monoxide level: ");
        double co = double.Parse(Console.ReadLine());
        
        data.Add(new AirQualityData
        {
            Timestamp = DateTime.Now,
            Location = location,
            PM25 = pm25,
            PM10 = pm10,
            Ozone = ozone,
            NO2 = no2,
            CO = co
        });
        
        Console.WriteLine("Reading added successfully.");
    }
    
    private void DisplayAllReadings(List<AirQualityData> data)
    {
        if (data.Count == 0)
        {
            Console.WriteLine("No readings available.");
            return;
        }
        
        foreach (var reading in data)
        {
            Console.WriteLine("\n--- Reading ---");
            Console.WriteLine("Timestamp: " + reading.Timestamp);
            Console.WriteLine("Location: " + reading.Location);
            Console.WriteLine("PM2.5: " + reading.PM25);
            Console.WriteLine("PM10: " + reading.PM10);
            Console.WriteLine("Ozone: " + reading.Ozone);
            Console.WriteLine("NO2: " + reading.NO2);
            Console.WriteLine("CO: " + reading.CO);
        }
    }
    
    private void AnalyzeData(List<AirQualityData> data)
    {
        if (data.Count == 0)
        {
            Console.WriteLine("No data to analyze.");
            return;
        }
        
        double avgPM25 = 0, avgPM10 = 0, avgOzone = 0, avgNO2 = 0, avgCO = 0;
        double maxPM25 = double.MinValue, maxPM10 = double.MinValue;
        
        foreach (var reading in data)
        {
            avgPM25 += reading.PM25;
            avgPM10 += reading.PM10;
            avgOzone += reading.Ozone;
            avgNO2 += reading.NO2;
            avgCO += reading.CO;
            
            if (reading.PM25 > maxPM25) maxPM25 = reading.PM25;
            if (reading.PM10 > maxPM10) maxPM10 = reading.PM10;
        }
        
        avgPM25 /= data.Count;
        avgPM10 /= data.Count;
        avgOzone /= data.Count;
        avgNO2 /= data.Count;
        avgCO /= data.Count;
        
        Console.WriteLine("\n--- Analysis Results ---");
        Console.WriteLine("Average PM2.5: " + avgPM25);
        Console.WriteLine("Average PM10: " + avgPM10);
        Console.WriteLine("Average Ozone: " + avgOzone);
        Console.WriteLine("Average NO2: " + avgNO2);
        Console.WriteLine("Average CO: " + avgCO);
        Console.WriteLine("Highest PM2.5 recorded: " + maxPM25);
        Console.WriteLine("Highest PM10 recorded: " + maxPM10);
    }
}

public class AirQualityData
{
    public DateTime Timestamp { get; set; }
    public string Location { get; set; }
    public double PM25 { get; set; }
    public double PM10 { get; set; }
    public double Ozone { get; set; }
    public double NO2 { get; set; }
    public double CO { get; set; }
}