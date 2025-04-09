using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class AccountGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Account Generator";
    private readonly Random _random = new Random();
    private const string UsernameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string PasswordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_+{}|:<>?-=[];,./";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Account Generator Module Started");
        
        try
        {
            string outputPath = Path.Combine(dataFolder, "generated_accounts.json");
            List<Account> accounts = new List<Account>();
            
            Console.WriteLine("Generating 10 random accounts...");
            
            for (int i = 0; i < 10; i++)
            {
                string username = GenerateRandomString(UsernameChars, 8);
                string password = GenerateRandomString(PasswordChars, 12);
                accounts.Add(new Account { Username = username, Password = password });
            }
            
            string json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(outputPath, json);
            
            Console.WriteLine("Accounts generated and saved to " + outputPath);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private string GenerateRandomString(string chars, int length)
    {
        char[] result = new char[length];
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[_random.Next(chars.Length)];
        }
        return new string(result);
    }
}

public class Account
{
    public string Username { get; set; }
    public string Password { get; set; }
}