using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";

    private const string ConfigFileName = "PasswordGeneratorConfig.json";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running.");

        try
        {
            var config = LoadConfig(dataFolder);
            if (config == null)
            {
                config = GetDefaultConfig();
                SaveConfig(dataFolder, config);
            }

            Console.WriteLine("Generating passwords with the following settings:");
            Console.WriteLine("Length: " + config.PasswordLength);
            Console.WriteLine("Include Numbers: " + config.IncludeNumbers);
            Console.WriteLine("Include Special Characters: " + config.IncludeSpecialChars);

            for (int i = 0; i < 5; i++)
            {
                string password = GeneratePassword(config);
                Console.WriteLine("Generated Password " + (i + 1) + ": " + password);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private PasswordGeneratorConfig LoadConfig(string dataFolder)
    {
        string configPath = Path.Combine(dataFolder, ConfigFileName);
        if (!File.Exists(configPath))
            return null;

        string json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<PasswordGeneratorConfig>(json);
    }

    private void SaveConfig(string dataFolder, PasswordGeneratorConfig config)
    {
        string configPath = Path.Combine(dataFolder, ConfigFileName);
        string json = JsonSerializer.Serialize(config);
        File.WriteAllText(configPath, json);
    }

    private PasswordGeneratorConfig GetDefaultConfig()
    {
        return new PasswordGeneratorConfig
        {
            PasswordLength = 12,
            IncludeNumbers = true,
            IncludeSpecialChars = true
        };
    }

    private string GeneratePassword(PasswordGeneratorConfig config)
    {
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{}|;:'\",.<>/?";

        var charSet = new StringBuilder(lowerChars + upperChars);

        if (config.IncludeNumbers)
            charSet.Append(numberChars);

        if (config.IncludeSpecialChars)
            charSet.Append(specialChars);

        var random = new Random();
        var password = new StringBuilder(config.PasswordLength);

        for (int i = 0; i < config.PasswordLength; i++)
        {
            password.Append(charSet[random.Next(charSet.Length)]);
        }

        return password.ToString();
    }
}

public class PasswordGeneratorConfig
{
    public int PasswordLength { get; set; }
    public bool IncludeNumbers { get; set; }
    public bool IncludeSpecialChars { get; set; }
}