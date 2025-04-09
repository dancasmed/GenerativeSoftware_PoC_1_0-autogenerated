using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CurrencyConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Currency Converter Module";

    private Dictionary<string, decimal> exchangeRates;

    public CurrencyConverterModule()
    {
        exchangeRates = new Dictionary<string, decimal>
        {
            {"USD_EUR", 0.85m},
            {"USD_GBP", 0.73m},
            {"USD_JPY", 110.25m},
            {"EUR_USD", 1.18m},
            {"GBP_USD", 1.37m},
            {"JPY_USD", 0.0091m}
        };
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Currency Converter Module is running...");
        Console.WriteLine("Available currencies: USD, EUR, GBP, JPY");

        try
        {
            string ratesFilePath = Path.Combine(dataFolder, "exchangeRates.json");
            if (File.Exists(ratesFilePath))
            {
                string jsonData = File.ReadAllText(ratesFilePath);
                exchangeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonData);
            }
            else
            {
                string jsonData = JsonSerializer.Serialize(exchangeRates);
                File.WriteAllText(ratesFilePath, jsonData);
            }

            while (true)
            {
                Console.Write("Enter source currency (or 'exit' to quit): ");
                string sourceCurrency = Console.ReadLine().ToUpper();

                if (sourceCurrency == "EXIT")
                    break;

                Console.Write("Enter target currency: ");
                string targetCurrency = Console.ReadLine().ToUpper();

                Console.Write("Enter amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Console.WriteLine("Invalid amount. Please try again.");
                    continue;
                }

                string rateKey = sourceCurrency + "_" + targetCurrency;
                if (exchangeRates.ContainsKey(rateKey))
                {
                    decimal convertedAmount = amount * exchangeRates[rateKey];
                    Console.WriteLine(amount + " " + sourceCurrency + " = " + convertedAmount + " " + targetCurrency);
                }
                else
                {
                    Console.WriteLine("Currency pair not supported.");
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
}