using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Password Generator Module...");

        try
        {
            bool exitRequested = false;
            while (!exitRequested)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Generate New Password");
                Console.WriteLine("2. Exit");
                Console.Write("Select an option: ");
                
                var input = Console.ReadLine()?.Trim();
                switch (input)
                {
                    case "1":
                        GenerateAndSavePassword(dataFolder);
                        break;
                    case "2":
                        exitRequested = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private void GenerateAndSavePassword(string dataFolder)
    {
        var criteria = GetPasswordCriteria();
        var password = GenerateSecurePassword(criteria);
        var strength = CalculatePasswordStrength(password, criteria);
        
        SavePasswordResult(dataFolder, new GeneratedPassword 
        { 
            password = password, 
            strength = strength, 
            timestamp = DateTime.UtcNow 
        });

        Console.WriteLine("Generated Password: " + password);
        Console.WriteLine("Password Strength: " + strength);
    }

    private PasswordCriteria GetPasswordCriteria()
    {
        return new PasswordCriteria
        {
            length = GetValidatedInt("Enter password length (8-64): ", 8, 64),
            include_special_chars = GetYesNoInput("Include special characters? (Y/N): "),
            include_numbers = GetYesNoInput("Include numbers? (Y/N): "),
            include_uppercase = GetYesNoInput("Include uppercase letters? (Y/N): "),
            include_lowercase = GetYesNoInput("Include lowercase letters? (Y/N): ")
        };
    }

    private int GetValidatedInt(string prompt, int min, int max)
    {
        int result;
        do
        {
            Console.Write(prompt);
        } while (!int.TryParse(Console.ReadLine(), out result) || result < min || result > max);
        return result;
    }

    private bool GetYesNoInput(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine().Trim().ToUpper() == "Y";
    }

    private string GenerateSecurePassword(PasswordCriteria criteria)
    {
        var charPool = new StringBuilder();
        var mandatoryChars = new List<char>();

        if (criteria.include_lowercase)
        {
            charPool.Append("abcdefghijklmnopqrstuvwxyz");
            mandatoryChars.Add(GetRandomChar("abcdefghijklmnopqrstuvwxyz"));
        }
        if (criteria.include_uppercase)
        {
            charPool.Append("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            mandatoryChars.Add(GetRandomChar("ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
        }
        if (criteria.include_numbers)
        {
            charPool.Append("0123456789");
            mandatoryChars.Add(GetRandomChar("0123456789"));
        }
        if (criteria.include_special_chars)
        {
            charPool.Append("!@#$%^&*()-_=+[]{};:'\",.<>/?");
            mandatoryChars.Add(GetRandomChar("!@#$%^&*()-_=+[]{};:'\",.<>/?"));
        }

        if (charPool.Length == 0)
            throw new InvalidOperationException("At least one character set must be selected");

        var remainingLength = criteria.length - mandatoryChars.Count;
        var passwordChars = new List<char>(mandatoryChars);

        for (int i = 0; i < remainingLength; i++)
            passwordChars.Add(GetRandomChar(charPool.ToString()));

        Shuffle(passwordChars);
        return new string(passwordChars.ToArray());
    }

    private char GetRandomChar(string characters)
    {
        using var rng = RandomNumberGenerator.Create();
        var data = new byte[4];
        rng.GetBytes(data);
        var randomValue = BitConverter.ToUInt32(data, 0);
        return characters[(int)(randomValue % (uint)characters.Length)];
    }

    private void Shuffle<T>(IList<T> list)
    {
        using var rng = RandomNumberGenerator.Create();
        int n = list.Count;
        while (n > 1)
        {
            byte[] box = new byte[4];
            do rng.GetBytes(box);
            while (!(BitConverter.ToUInt32(box, 0) < n * (uint.MaxValue / n)));
            int k = (int)(BitConverter.ToUInt32(box, 0) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private string CalculatePasswordStrength(string password, PasswordCriteria criteria)
    {
        int complexityFactors = 0;
        if (criteria.include_lowercase) complexityFactors++;
        if (criteria.include_uppercase) complexityFactors++;
        if (criteria.include_numbers) complexityFactors++;
        if (criteria.include_special_chars) complexityFactors++;

        return password.Length switch
        {
            >= 16 when complexityFactors >= 4 => "Very Strong",
            >= 12 when complexityFactors >= 3 => "Strong",
            >= 8 when complexityFactors >= 2 => "Medium",
            _ => "Weak"
        };
    }

    private void SavePasswordResult(string dataFolder, GeneratedPassword result)
    {
        Directory.CreateDirectory(dataFolder);
        var historyFile = Path.Combine(dataFolder, "password_history.json");
        
        var history = File.Exists(historyFile)
            ? JsonSerializer.Deserialize<PasswordHistory>(File.ReadAllText(historyFile))
            : new PasswordHistory { passwords = new List<GeneratedPassword>() };

        history.passwords.Add(result);
        File.WriteAllText(historyFile, JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true }));
    }
}

public class PasswordCriteria
{
    public int length { get; set; }
    public bool include_special_chars { get; set; }
    public bool include_numbers { get; set; }
    public bool include_uppercase { get; set; }
    public bool include_lowercase { get; set; }
}

public class GeneratedPassword
{
    public string password { get; set; }
    public string strength { get; set; }
    public DateTime timestamp { get; set; }
}

public class PasswordHistory
{
    public List<GeneratedPassword> passwords { get; set; } = new List<GeneratedPassword>();
    public string user_id { get; set; } = "system";
}