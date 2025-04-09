using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class LotteryProbabilityCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Lottery Probability Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Lottery Probability Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "lottery_config.json");
            LotteryConfig config;
            
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<LotteryConfig>(json);
            }
            else
            {
                config = new LotteryConfig
                {
                    TotalNumbers = 49,
                    NumbersDrawn = 6,
                    BonusNumbers = 1
                };
                
                string json = JsonSerializer.Serialize(config);
                File.WriteAllText(configPath, json);
            }
            
            double mainProbability = CalculateCombinationProbability(config.TotalNumbers, config.NumbersDrawn);
            double bonusProbability = config.BonusNumbers > 0 
                ? CalculateCombinationProbabilityWithBonus(config.TotalNumbers, config.NumbersDrawn, config.BonusNumbers)
                : 0;
            
            Console.WriteLine("Probability of winning the main prize: 1 in " + mainProbability);
            if (config.BonusNumbers > 0)
            {
                Console.WriteLine("Probability of winning with bonus: 1 in " + bonusProbability);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateCombinationProbability(int totalNumbers, int numbersDrawn)
    {
        return Combination(totalNumbers, numbersDrawn);
    }
    
    private double CalculateCombinationProbabilityWithBonus(int totalNumbers, int numbersDrawn, int bonusNumbers)
    {
        return Combination(totalNumbers, numbersDrawn) * Combination(totalNumbers - numbersDrawn, bonusNumbers);
    }
    
    private double Combination(int n, int k)
    {
        if (k > n) return 0;
        if (k == 0 || k == n) return 1;
        
        double result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= (double)(n - k + i) / i;
        }
        
        return result;
    }
}

public class LotteryConfig
{
    public int TotalNumbers { get; set; }
    public int NumbersDrawn { get; set; }
    public int BonusNumbers { get; set; }
}