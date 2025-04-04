using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";
    
    private const string ConfigFileName = "password_config.json";
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running.");
        
        try
        {
            var config = LoadOrCreateConfig(dataFolder);
            
            Console.WriteLine("Generating passwords with the following settings:");
            Console.WriteLine("Length: " + config.Length);
            Console.WriteLine("Include Special Characters: " + config.IncludeSpecialCharacters);
            Console.WriteLine("Number of Passwords: " + config.NumberOfPasswords);
            
            for (int i = 0; i < config.NumberOfPasswords; i++)
            {
                string password = GeneratePassword(config.Length, config.IncludeSpecialCharacters);
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
    
    private PasswordConfig LoadOrCreateConfig(string dataFolder)
    {
        string configPath = Path.Combine(dataFolder, ConfigFileName);
        
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<PasswordConfig>(json);
        }
        else
        {
            var defaultConfig = new PasswordConfig
            {
                Length = 12,
                IncludeSpecialCharacters = true,
                NumberOfPasswords = 5
            };
            
            string json = JsonSerializer.Serialize(defaultConfig);
            Directory.CreateDirectory(dataFolder);
            File.WriteAllText(configPath, json);
            
            return defaultConfig;
        }
    }
    
    private string GeneratePassword(int length, bool includeSpecialChars)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        const string specialChars = "!@#$%^&*()_-+=[{]};:<>|./?";
        
        var chars = new StringBuilder(validChars);
        if (includeSpecialChars)
        {
            chars.Append(specialChars);
        }
        
        var random = new Random();
        var password = new StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }
        
        return password.ToString();
    }
}

public class PasswordConfig
{
    public int Length { get; set; }
    public bool IncludeSpecialCharacters { get; set; }
    public int NumberOfPasswords { get; set; }
}