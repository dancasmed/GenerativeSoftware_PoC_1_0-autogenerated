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
        
        try
        {
            string optionsFilePath = Path.Combine(dataFolder, "passwordOptions.json");
            PasswordOptions options;
            
            if (File.Exists(optionsFilePath))
            {
                string json = File.ReadAllText(optionsFilePath);
                options = JsonSerializer.Deserialize<PasswordOptions>(json);
            }
            else
            {
                options = new PasswordOptions();
                string defaultJson = JsonSerializer.Serialize(options);
                File.WriteAllText(optionsFilePath, defaultJson);
            }
            
            string password = GeneratePassword(options.Length, options.IncludeSpecialChars);
            Console.WriteLine("Generated Password: " + password);
            
            string outputPath = Path.Combine(dataFolder, "generated_password.txt");
            File.WriteAllText(outputPath, password);
            
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
        const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
        const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string numberChars = "0123456789";
        const string specialChars = "!@#$%^&*()-_=+[]{};:'\",.<>/?\\|`~";
        
        StringBuilder validChars = new StringBuilder();
        validChars.Append(lowerChars).Append(upperChars).Append(numberChars);
        
        if (includeSpecialChars)
        {
            validChars.Append(specialChars);
        }
        
        Random random = new Random();
        StringBuilder password = new StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            password.Append(validChars[random.Next(validChars.Length)]);
        }
        
        return password.ToString();
    }
}