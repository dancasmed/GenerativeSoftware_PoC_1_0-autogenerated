using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class StockAnalyzer : IGeneratedModule
{
    public string Name { get; set; } = "Stock Market Trend Analyzer";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Stock Market Trend Analyzer is running...");

        try
        {
            // Ensure data directory exists
            Directory.CreateDirectory(dataFolder);

            // Load or create sample stock data
            var stocks = LoadOrCreateSampleData(dataFolder);

            // Analyze trends
            var recommendations = AnalyzeTrends(stocks);

            // Save recommendations
            SaveRecommendations(dataFolder, recommendations);

            // Display summary
            DisplaySummary(recommendations);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred: " + ex.Message);
            return false;
        }
    }

    private List<StockData> LoadOrCreateSampleData(string dataFolder)
    {
        string dataFile = Path.Combine(dataFolder, "stock_data.json");

        if (File.Exists(dataFile))
        {
            var json = File.ReadAllText(dataFile);
            return JsonSerializer.Deserialize<List<StockData>>(json);
        }
        else
        {
            // Create sample data
            var sampleData = new List<StockData>
            {
                new StockData { Symbol = "AAPL", Name = "Apple Inc.", CurrentPrice = 175.50m, MovingAverage = 170.25m, RSI = 65, Volume = 75000000 },
                new StockData { Symbol = "MSFT", Name = "Microsoft Corp.", CurrentPrice = 320.75m, MovingAverage = 315.40m, RSI = 58, Volume = 45000000 },
                new StockData { Symbol = "GOOGL", Name = "Alphabet Inc.", CurrentPrice = 145.20m, MovingAverage = 142.80m, RSI = 62, Volume = 30000000 },
                new StockData { Symbol = "AMZN", Name = "Amazon.com Inc.", CurrentPrice = 185.30m, MovingAverage = 180.90m, RSI = 70, Volume = 60000000 },
                new StockData { Symbol = "TSLA", Name = "Tesla Inc.", CurrentPrice = 210.45m, MovingAverage = 220.10m, RSI = 45, Volume = 90000000 }
            };

            var json = JsonSerializer.Serialize(sampleData);
            File.WriteAllText(dataFile, json);

            return sampleData;
        }
    }

    private List<StockRecommendation> AnalyzeTrends(List<StockData> stocks)
    {
        var recommendations = new List<StockRecommendation>();

        foreach (var stock in stocks)
        {
            string recommendation;
            string reason;

            // Simple analysis logic
            if (stock.CurrentPrice > stock.MovingAverage * 1.05m && stock.RSI > 70)
            {
                recommendation = "SELL";
                reason = "Overbought condition (price above MA and RSI > 70)";
            }
            else if (stock.CurrentPrice < stock.MovingAverage * 0.95m && stock.RSI < 30)
            {
                recommendation = "BUY";
                reason = "Oversold condition (price below MA and RSI < 30)";
            }
            else if (stock.CurrentPrice > stock.MovingAverage && stock.Volume > 50000000)
            {
                recommendation = "BUY";
                reason = "Bullish trend with high volume";
            }
            else if (stock.CurrentPrice < stock.MovingAverage && stock.Volume > 50000000)
            {
                recommendation = "SELL";
                reason = "Bearish trend with high volume";
            }
            else
            {
                recommendation = "HOLD";
                reason = "Neutral conditions";
            }

            recommendations.Add(new StockRecommendation
            {
                Symbol = stock.Symbol,
                Name = stock.Name,
                Recommendation = recommendation,
                Reason = reason,
                CurrentPrice = stock.CurrentPrice,
                MovingAverage = stock.MovingAverage,
                RSI = stock.RSI,
                Volume = stock.Volume
            });
        }

        return recommendations;
    }

    private void SaveRecommendations(string dataFolder, List<StockRecommendation> recommendations)
    {
        string recFile = Path.Combine(dataFolder, "stock_recommendations.json");
        var json = JsonSerializer.Serialize(recommendations);
        File.WriteAllText(recFile, json);
    }

    private void DisplaySummary(List<StockRecommendation> recommendations)
    {
        Console.WriteLine("\nStock Recommendations:");
        Console.WriteLine("----------------------");

        foreach (var rec in recommendations)
        {
            Console.WriteLine("Symbol: " + rec.Symbol);
            Console.WriteLine("Name: " + rec.Name);
            Console.WriteLine("Current Price: " + rec.CurrentPrice);
            Console.WriteLine("50-Day MA: " + rec.MovingAverage);
            Console.WriteLine("RSI: " + rec.RSI);
            Console.WriteLine("Volume: " + rec.Volume);
            Console.WriteLine("Recommendation: " + rec.Recommendation);
            Console.WriteLine("Reason: " + rec.Reason);
            Console.WriteLine();
        }
    }
}

public class StockData
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal MovingAverage { get; set; }
    public int RSI { get; set; }
    public long Volume { get; set; }
}

public class StockRecommendation
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public string Recommendation { get; set; }
    public string Reason { get; set; }
    public decimal CurrentPrice { get; set; }
    public decimal MovingAverage { get; set; }
    public int RSI { get; set; }
    public long Volume { get; set; }
}