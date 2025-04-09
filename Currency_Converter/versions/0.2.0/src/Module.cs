using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class CurrencyConverter : IGeneratedModule
{
    public string Name { get; set; } = "Currency Converter";
    private HttpClient _httpClient;
    private Dictionary<string, decimal> _exchangeRates;
    private string _ratesFilePath;

    public CurrencyConverter()
    {
        _httpClient = new HttpClient();
        _exchangeRates = new Dictionary<string, decimal>();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Currency Converter Module is running...");
        _ratesFilePath = Path.Combine(dataFolder, "exchangeRates.json");

        try
        {
            LoadExchangeRates().Wait();
            Console.WriteLine("Available currencies: USD, EUR, GBP, JPY, AUD");
            Console.WriteLine("Enter source currency (e.g., USD):");
            string fromCurrency = Console.ReadLine().ToUpper();
            Console.WriteLine("Enter target currency (e.g., EUR):");
            string toCurrency = Console.ReadLine().ToUpper();
            Console.WriteLine("Enter amount to convert:");
            decimal amount = decimal.Parse(Console.ReadLine());

            decimal convertedAmount = ConvertCurrency(fromCurrency, toCurrency, amount);
            Console.WriteLine("{0} {1} = {2} {3}", amount, fromCurrency, convertedAmount, toCurrency);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private async Task LoadExchangeRates()
    {
        if (File.Exists(_ratesFilePath) && (DateTime.Now - File.GetLastWriteTime(_ratesFilePath)).TotalHours < 1)
        {
            string json = File.ReadAllText(_ratesFilePath);
            _exchangeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
        }
        else
        {
            try
            {
                string apiResponse = await _httpClient.GetStringAsync("https://api.exchangerate-api.com/v4/latest/USD");
                using JsonDocument doc = JsonDocument.Parse(apiResponse);
                JsonElement rates = doc.RootElement.GetProperty("rates");

                _exchangeRates.Clear();
                foreach (JsonProperty rate in rates.EnumerateObject())
                {
                    _exchangeRates[rate.Name] = rate.Value.GetDecimal();
                }

                string json = JsonSerializer.Serialize(_exchangeRates);
                File.WriteAllText(_ratesFilePath, json);
            }
            catch (HttpRequestException)
            {
                Console.WriteLine("Warning: Could not fetch latest rates. Using cached data if available.");
                if (File.Exists(_ratesFilePath))
                {
                    string json = File.ReadAllText(_ratesFilePath);
                    _exchangeRates = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json);
                }
                else
                {
                    throw new Exception("No exchange rate data available");
                }
            }
        }
    }

    private decimal ConvertCurrency(string fromCurrency, string toCurrency, decimal amount)
    {
        if (!_exchangeRates.ContainsKey(fromCurrency) || !_exchangeRates.ContainsKey(toCurrency))
        {
            throw new Exception("One or both currencies are not supported");
        }

        decimal fromRate = _exchangeRates[fromCurrency];
        decimal toRate = _exchangeRates[toCurrency];
        return amount * (toRate / fromRate);
    }
}