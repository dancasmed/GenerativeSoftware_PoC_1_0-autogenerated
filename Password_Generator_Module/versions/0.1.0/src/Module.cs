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

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running...");
        Console.WriteLine("Generating random passwords with customizable options.");

        try
        {
            string configPath = Path.Combine(dataFolder, "password_config.json");
            PasswordOptions options;

            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                options = JsonSerializer.Deserialize<PasswordOptions>(json);
                Console.WriteLine("Loaded configuration from file.");
            }
            else
            {
                options = new PasswordOptions();
                string defaultJson = JsonSerializer.Serialize(options);
                File.WriteAllText(configPath, defaultJson);
                Console.WriteLine("Created default configuration file.");
            }

            string password = GeneratePassword(options.Length, options.IncludeSpecialChars);
            Console.WriteLine("Generated Password: " + password);

            string outputPath = Path.Combine(dataFolder, "generated_password.txt");
            File.WriteAllText(outputPath, password);
            Console.WriteLine("Password saved to: " + outputPath);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private string GeneratePassword(int length, bool includeSpecialChars)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        StringBuilder chars = new StringBuilder(validChars);
        if (includeSpecialChars)
        {
            chars.Append(specialChars);
        }

        Random random = new Random();
        StringBuilder password = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }

        return password.ToString();
    }
}