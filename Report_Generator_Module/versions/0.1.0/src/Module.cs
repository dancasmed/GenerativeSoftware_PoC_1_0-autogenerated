using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class ReportGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Report Generator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Report Generator Module is running...");
        
        try
        {
            string rawDataPath = Path.Combine(dataFolder, "raw_data.json");
            string reportPath = Path.Combine(dataFolder, "generated_report.txt");
            
            if (!File.Exists(rawDataPath))
            {
                Console.WriteLine("No raw data found. Generating sample data...");
                GenerateSampleData(rawDataPath);
            }
            
            List<DataItem> dataItems = LoadDataItems(rawDataPath);
            string report = GenerateReport(dataItems);
            
            File.WriteAllText(reportPath, report);
            Console.WriteLine("Report generated successfully at: " + reportPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating report: " + ex.Message);
            return false;
        }
    }
    
    private void GenerateSampleData(string filePath)
    {
        var sampleData = new List<DataItem>
        {
            new DataItem { Id = 1, Name = "Item 1", Value = 100, Timestamp = DateTime.Now.AddDays(-1) },
            new DataItem { Id = 2, Name = "Item 2", Value = 200, Timestamp = DateTime.Now.AddHours(-12) },
            new DataItem { Id = 3, Name = "Item 3", Value = 300, Timestamp = DateTime.Now }
        };
        
        string json = JsonSerializer.Serialize(sampleData);
        File.WriteAllText(filePath, json);
    }
    
    private List<DataItem> LoadDataItems(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<DataItem>>(json);
    }
    
    private string GenerateReport(List<DataItem> dataItems)
    {
        var report = new System.Text.StringBuilder();
        report.AppendLine("CUSTOM DATA REPORT");
        report.AppendLine("Generated on: " + DateTime.Now.ToString());
        report.AppendLine();
        report.AppendLine("ID\tName\tValue\tTimestamp");
        
        foreach (var item in dataItems)
        {
            report.AppendLine(item.Id + "\t" + item.Name + "\t" + item.Value + "\t" + item.Timestamp);
        }
        
        report.AppendLine();
        report.AppendLine("SUMMARY STATISTICS");
        report.AppendLine("Total Items: " + dataItems.Count);
        report.AppendLine("Average Value: " + (dataItems.Sum(i => i.Value) / dataItems.Count));
        report.AppendLine("Max Value: " + dataItems.Max(i => i.Value));
        report.AppendLine("Min Value: " + dataItems.Min(i => i.Value));
        
        return report.ToString();
    }
}

public class DataItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Value { get; set; }
    public DateTime Timestamp { get; set; }
}