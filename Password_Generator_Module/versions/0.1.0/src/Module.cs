using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";

    private class PasswordOptions
    {
        public int Length { get; set; } = 12;
        public bool IncludeSpecialChars { get; set; } = true;
    }

    private const string OptionsFileName = "password_options.json";
    private readonly Random _random = new Random();
    private const string LowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string DigitChars = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_+-=[]{};':\"\\|,.<>/?";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running...");
        Console.WriteLine("Generating random passwords with customizable options.");

        try
        {
            var options = LoadOptions(dataFolder);
            Console.WriteLine("Current settings: Length = " + options.Length + ", Special Characters = " + (options.IncludeSpecialChars ? "Enabled" : "Disabled"));

            Console.WriteLine("Generated Password: " + GeneratePassword(options));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private PasswordOptions LoadOptions(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, OptionsFileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<PasswordOptions>(json) ?? new PasswordOptions();
        }

        var defaultOptions = new PasswordOptions();
        SaveOptions(dataFolder, defaultOptions);
        return defaultOptions;
    }

    private void SaveOptions(string dataFolder, PasswordOptions options)
    {
        string filePath = Path.Combine(dataFolder, OptionsFileName);
        string json = JsonSerializer.Serialize(options);
        File.WriteAllText(filePath, json);
    }

    private string GeneratePassword(PasswordOptions options)
    {
        var chars = new StringBuilder();
        chars.Append(LowerCaseChars).Append(UpperCaseChars).Append(DigitChars);

        if (options.IncludeSpecialChars)
        {
            chars.Append(SpecialChars);
        }

        var password = new StringBuilder();
        for (int i = 0; i < options.Length; i++)
        {
            password.Append(chars.ToString()[_random.Next(chars.Length)]);
        }

        return password.ToString();
    }
}