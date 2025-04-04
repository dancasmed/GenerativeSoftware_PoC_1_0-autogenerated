using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class TipCalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "Tip Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Tip Calculator Module is running...");

        try
        {
            string settingsPath = Path.Combine(dataFolder, "tip_settings.json");
            TipSettings settings = LoadSettings(settingsPath);

            Console.WriteLine("Enter the total bill amount:");
            decimal billAmount = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter the tip percentage (e.g., 15 for 15%):");
            decimal tipPercentage = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Enter the number of people to split the bill:");
            int numberOfPeople = int.Parse(Console.ReadLine());

            decimal tipAmount = CalculateTip(billAmount, tipPercentage);
            decimal totalAmount = billAmount + tipAmount;
            decimal amountPerPerson = totalAmount / numberOfPeople;

            Console.WriteLine("Bill Summary:");
            Console.WriteLine("Total Bill: " + billAmount.ToString("C"));
            Console.WriteLine("Tip Percentage: " + tipPercentage + "%");
            Console.WriteLine("Tip Amount: " + tipAmount.ToString("C"));
            Console.WriteLine("Total Amount: " + totalAmount.ToString("C"));
            Console.WriteLine("Amount per person: " + amountPerPerson.ToString("C"));

            settings.LastBillAmount = billAmount;
            settings.LastTipPercentage = tipPercentage;
            SaveSettings(settingsPath, settings);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private decimal CalculateTip(decimal billAmount, decimal tipPercentage)
    {
        return billAmount * (tipPercentage / 100);
    }

    private TipSettings LoadSettings(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<TipSettings>(json);
        }
        return new TipSettings();
    }

    private void SaveSettings(string filePath, TipSettings settings)
    {
        string json = JsonSerializer.Serialize(settings);
        File.WriteAllText(filePath, json);
    }
}

public class TipSettings
{
    public decimal LastBillAmount { get; set; } = 0;
    public decimal LastTipPercentage { get; set; } = 15;
}