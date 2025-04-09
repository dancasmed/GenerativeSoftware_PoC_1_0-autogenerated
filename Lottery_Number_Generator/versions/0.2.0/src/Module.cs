using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class LotteryNumberGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Lottery Number Generator";
    
    private Random _random;
    private string _configFilePath;
    private LotteryConfig _config;
    
    public LotteryNumberGenerator()
    {
        _random = new Random();
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Lottery Number Generator Module is running...");
        
        _configFilePath = Path.Combine(dataFolder, "lottery_config.json");
        
        try
        {
            LoadOrCreateConfig();
            
            Console.WriteLine("Generating lottery numbers...");
            Console.WriteLine("Game: " + _config.GameName);
            Console.WriteLine("Numbers per ticket: " + _config.NumbersPerTicket);
            Console.WriteLine("Maximum number: " + _config.MaxNumber);
            
            int[] numbers = GenerateNumbers();
            
            Console.WriteLine("Your numbers are: " + string.Join(", ", numbers));
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void LoadOrCreateConfig()
    {
        if (File.Exists(_configFilePath))
        {
            string json = File.ReadAllText(_configFilePath);
            _config = JsonSerializer.Deserialize<LotteryConfig>(json);
        }
        else
        {
            _config = new LotteryConfig
            {
                GameName = "Powerball",
                NumbersPerTicket = 6,
                MaxNumber = 69
            };
            
            string json = JsonSerializer.Serialize(_config);
            File.WriteAllText(_configFilePath, json);
        }
    }
    
    private int[] GenerateNumbers()
    {
        int[] numbers = new int[_config.NumbersPerTicket];
        
        for (int i = 0; i < _config.NumbersPerTicket; i++)
        {
            numbers[i] = _random.Next(1, _config.MaxNumber + 1);
        }
        
        Array.Sort(numbers);
        return numbers;
    }
}

public class LotteryConfig
{
    public string GameName { get; set; }
    public int NumbersPerTicket { get; set; }
    public int MaxNumber { get; set; }
}