using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CurrencyDenominationModule : IGeneratedModule
{
    public string Name { get; set; } = "Currency Denomination Calculator";
    
    private Dictionary<decimal, int> _denominations = new Dictionary<decimal, int>()
    {
        { 100m, 0 },
        { 50m, 0 },
        { 20m, 0 },
        { 10m, 0 },
        { 5m, 0 },
        { 1m, 0 },
        { 0.25m, 0 },
        { 0.10m, 0 },
        { 0.05m, 0 },
        { 0.01m, 0 }
    };

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Currency Denomination Calculator is running...");
        
        string configPath = Path.Combine(dataFolder, "denominations_config.json");
        
        try
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                _denominations = JsonSerializer.Deserialize<Dictionary<decimal, int>>(json);
            }
            else
            {
                SaveConfig(configPath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading configuration: " + ex.Message);
            return false;
        }

        Console.WriteLine("Enter the amount to break down:");
        string input = Console.ReadLine();
        
        if (!decimal.TryParse(input, out decimal amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount. Please enter a positive decimal value.");
            return false;
        }

        Dictionary<decimal, int> breakdown = CalculateBreakdown(amount);
        
        Console.WriteLine("\nCurrency Breakdown:");
        foreach (var denomination in breakdown)
        {
            if (denomination.Value > 0)
            {
                Console.WriteLine(denomination.Key.ToString("C") + ": " + denomination.Value);
            }
        }
        
        return true;
    }
    
    private Dictionary<decimal, int> CalculateBreakdown(decimal amount)
    {
        Dictionary<decimal, int> result = new Dictionary<decimal, int>();
        decimal remaining = amount;
        
        foreach (decimal denomination in _denominations.Keys)
        {
            if (remaining <= 0) break;
            
            int count = (int)(remaining / denomination);
            if (count > 0)
            {
                result[denomination] = count;
                remaining -= count * denomination;
                remaining = Math.Round(remaining, 2);
            }
        }
        
        return result;
    }
    
    private void SaveConfig(string configPath)
    {
        try
        {
            string json = JsonSerializer.Serialize(_denominations);
            File.WriteAllText(configPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving configuration: " + ex.Message);
        }
    }
}