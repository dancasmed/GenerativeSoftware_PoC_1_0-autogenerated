using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CurrencyConverterModule : IGeneratedModule
{
    public string Name { get; set; } = "Currency Converter";
    
    private Dictionary<string, decimal> _exchangeRates;
    private const string ExchangeRatesFileName = "exchange_rates.json";
    
    public CurrencyConverterModule()
    {
        _exchangeRates = new Dictionary<string, decimal>();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Currency Converter Module is running...");
        
        string exchangeRatesPath = Path.Combine(dataFolder, ExchangeRatesFileName);
        
        try
        {
            if (File.Exists(exchangeRatesPath))
            {
                string jsonData = File.ReadAllText(exchangeRatesPath);
                _exchangeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(jsonData);
                Console.WriteLine("Exchange rates loaded successfully.");
            }
            else
            {
                Console.WriteLine("No exchange rates file found. Starting with empty rates.");
            }
            
            bool running = true;
            while (running)
            {
                Console.WriteLine("\nOptions:");
                Console.WriteLine("1. Add/Update exchange rate");
                Console.WriteLine("2. Convert currency");
                Console.WriteLine("3. View all exchange rates");
                Console.WriteLine("4. Save and exit");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddUpdateExchangeRate();
                        break;
                    case "2":
                        ConvertCurrency();
                        break;
                    case "3":
                        ViewAllExchangeRates();
                        break;
                    case "4":
                        SaveExchangeRates(exchangeRatesPath);
                        running = false;
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
    
    private void AddUpdateExchangeRate()
    {
        Console.Write("Enter currency code (e.g., USD, EUR): ");
        string currencyCode = Console.ReadLine().ToUpper();
        
        Console.Write("Enter exchange rate (1 {0} equals how much in base currency): ", currencyCode);
        if (decimal.TryParse(Console.ReadLine(), out decimal rate))
        {
            _exchangeRates[currencyCode] = rate;
            Console.WriteLine("Exchange rate for {0} updated to {1}.", currencyCode, rate);
        }
        else
        {
            Console.WriteLine("Invalid rate entered.");
        }
    }
    
    private void ConvertCurrency()
    {
        if (_exchangeRates.Count == 0)
        {
            Console.WriteLine("No exchange rates available. Please add rates first.");
            return;
        }
        
        Console.Write("Enter source currency code: ");
        string sourceCurrency = Console.ReadLine().ToUpper();
        
        if (!_exchangeRates.ContainsKey(sourceCurrency))
        {
            Console.WriteLine("Exchange rate for {0} not found.", sourceCurrency);
            return;
        }
        
        Console.Write("Enter target currency code: ");
        string targetCurrency = Console.ReadLine().ToUpper();
        
        if (!_exchangeRates.ContainsKey(targetCurrency))
        {
            Console.WriteLine("Exchange rate for {0} not found.", targetCurrency);
            return;
        }
        
        Console.Write("Enter amount to convert: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount entered.");
            return;
        }
        
        decimal sourceRate = _exchangeRates[sourceCurrency];
        decimal targetRate = _exchangeRates[targetCurrency];
        
        decimal convertedAmount = (amount / sourceRate) * targetRate;
        
        Console.WriteLine("{0} {1} = {2} {3}", amount, sourceCurrency, convertedAmount, targetCurrency);
    }
    
    private void ViewAllExchangeRates()
    {
        if (_exchangeRates.Count == 0)
        {
            Console.WriteLine("No exchange rates available.");
            return;
        }
        
        Console.WriteLine("\nCurrent Exchange Rates:");
        foreach (var rate in _exchangeRates)
        {
            Console.WriteLine("{0}: {1}", rate.Key, rate.Value);
        }
    }
    
    private void SaveExchangeRates(string filePath)
    {
        try
        {
            string jsonData = JsonSerializer.Serialize(_exchangeRates);
            File.WriteAllText(filePath, jsonData);
            Console.WriteLine("Exchange rates saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving exchange rates: " + ex.Message);
        }
    }
}