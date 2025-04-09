using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";

    private const string ConfigFileName = "password_config.json";
    private const string GeneratedPasswordsFileName = "generated_passwords.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running...");

        try
        {
            var config = LoadOrCreateConfig(dataFolder);
            Console.WriteLine("Current configuration:");
            DisplayConfig(config);

            Console.WriteLine("\nGenerating passwords...");
            var passwords = GeneratePasswords(config);

            SavePasswords(dataFolder, passwords);
            Console.WriteLine("Passwords generated and saved successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private PasswordConfig LoadOrCreateConfig(string dataFolder)
    {
        var configPath = Path.Combine(dataFolder, ConfigFileName);

        if (File.Exists(configPath))
        {
            var json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<PasswordConfig>(json);
        }
        else
        {
            var defaultConfig = new PasswordConfig
            {
                Length = 12,
                IncludeUppercase = true,
                IncludeNumbers = true,
                IncludeSpecialChars = true,
                PasswordCount = 5
            };

            var json = JsonSerializer.Serialize(defaultConfig);
            File.WriteAllText(configPath, json);

            return defaultConfig;
        }
    }

    private void DisplayConfig(PasswordConfig config)
    {
        Console.WriteLine("Password Length: " + config.Length);
        Console.WriteLine("Include Uppercase: " + config.IncludeUppercase);
        Console.WriteLine("Include Numbers: " + config.IncludeNumbers);
        Console.WriteLine("Include Special Characters: " + config.IncludeSpecialChars);
        Console.WriteLine("Number of Passwords: " + config.PasswordCount);
    }

    private string[] GeneratePasswords(PasswordConfig config)
    {
        var passwords = new string[config.PasswordCount];
        var random = new Random();
        var passwordChars = new char[config.Length];

        for (int i = 0; i < config.PasswordCount; i++)
        {
            var charSet = GetCharacterSet(config);

            for (int j = 0; j < config.Length; j++)
            {
                passwordChars[j] = charSet[random.Next(charSet.Length)];
            }

            passwords[i] = new string(passwordChars);
            Console.WriteLine("Generated Password " + (i + 1) + ": " + passwords[i]);
        }

        return passwords;
    }

    private string GetCharacterSet(PasswordConfig config)
    {
        var builder = new StringBuilder("abcdefghijklmnopqrstuvwxyz");

        if (config.IncludeUppercase)
            builder.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        if (config.IncludeNumbers)
            builder.Append("0123456789");

        if (config.IncludeSpecialChars)
            builder.Append("!@#$%^&*()_+-=[]{};':\"\\|,.<>/?");

        return builder.ToString();
    }

    private void SavePasswords(string dataFolder, string[] passwords)
    {
        var passwordsPath = Path.Combine(dataFolder, GeneratedPasswordsFileName);
        var json = JsonSerializer.Serialize(passwords);
        File.WriteAllText(passwordsPath, json);
    }

    private class PasswordConfig
    {
        public int Length { get; set; }
        public bool IncludeUppercase { get; set; }
        public bool IncludeNumbers { get; set; }
        public bool IncludeSpecialChars { get; set; }
        public int PasswordCount { get; set; }
    }
}