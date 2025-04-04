using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LotteryNumberGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Lottery Number Generator";
    
    private List<int> _userPicks = new List<int>();
    private Random _random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Lottery Number Generator Module is running...");
        
        string userPicksFilePath = Path.Combine(dataFolder, "user_picks.json");
        string resultsFilePath = Path.Combine(dataFolder, "lottery_results.json");
        
        LoadUserPicks(userPicksFilePath);
        
        if (_userPicks.Count == 0)
        {
            Console.WriteLine("No user picks found. Generating random picks.");
            GenerateRandomUserPicks();
            SaveUserPicks(userPicksFilePath);
        }
        
        List<int> winningNumbers = GenerateWinningNumbers();
        
        var results = new
        {
            UserPicks = _userPicks,
            WinningNumbers = winningNumbers,
            Matches = FindMatches(_userPicks, winningNumbers),
            Timestamp = DateTime.Now
        };
        
        SaveResults(resultsFilePath, results);
        
        DisplayResults(results);
        
        return true;
    }
    
    private void LoadUserPicks(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                _userPicks = JsonSerializer.Deserialize<List<int>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading user picks: " + ex.Message);
            }
        }
    }
    
    private void GenerateRandomUserPicks()
    {
        _userPicks.Clear();
        
        while (_userPicks.Count < 6)
        {
            int num = _random.Next(1, 50);
            if (!_userPicks.Contains(num))
            {
                _userPicks.Add(num);
            }
        }
        
        _userPicks.Sort();
    }
    
    private void SaveUserPicks(string filePath)
    {
        try
        {
            string json = JsonSerializer.Serialize(_userPicks);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving user picks: " + ex.Message);
        }
    }
    
    private List<int> GenerateWinningNumbers()
    {
        List<int> winningNumbers = new List<int>();
        
        while (winningNumbers.Count < 6)
        {
            int num = _random.Next(1, 50);
            if (!winningNumbers.Contains(num))
            {
                winningNumbers.Add(num);
            }
        }
        
        winningNumbers.Sort();
        return winningNumbers;
    }
    
    private List<int> FindMatches(List<int> userPicks, List<int> winningNumbers)
    {
        List<int> matches = new List<int>();
        
        foreach (int num in userPicks)
        {
            if (winningNumbers.Contains(num))
            {
                matches.Add(num);
            }
        }
        
        return matches;
    }
    
    private void SaveResults(string filePath, object results)
    {
        try
        {
            string json = JsonSerializer.Serialize(results);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving results: " + ex.Message);
        }
    }
    
    private void DisplayResults(dynamic results)
    {
        Console.WriteLine("\nLottery Results:");
        Console.WriteLine("User Picks: " + string.Join(", ", results.UserPicks));
        Console.WriteLine("Winning Numbers: " + string.Join(", ", results.WinningNumbers));
        Console.WriteLine("Matches: " + (results.Matches.Count > 0 ? string.Join(", ", results.Matches) : "None"));
        Console.WriteLine("Generated at: " + results.Timestamp);
    }
}