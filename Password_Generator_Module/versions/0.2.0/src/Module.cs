using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

public class PasswordGeneratorModule : IGeneratedModule
{
    public string Name { get; set; } = "Password Generator Module";
    
    private readonly string[] adjectives = 
    {
        "happy", "sunny", "brave", "quick", "lucky", "gentle", "calm", "bright", "clever", "fierce"
    };
    
    private readonly string[] nouns = 
    {
        "dog", "cat", "bird", "tree", "moon", "star", "river", "mountain", "ocean", "flower"
    };
    
    private readonly string[] separators = 
    {
        "!", "@", "#", "$", "%", "^", "&", "*", "-", "_", "+", "="
    };
    
    private readonly Random random = new Random();
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Password Generator Module is running...");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "password_config.json");
            PasswordConfig config = LoadConfig(configPath);
            
            string password = GenerateMemorablePassword(config);
            Console.WriteLine("Generated Password: " + password);
            
            SavePasswordHistory(dataFolder, password);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private PasswordConfig LoadConfig(string configPath)
    {
        if (File.Exists(configPath))
        {
            string json = File.ReadAllText(configPath);
            return JsonSerializer.Deserialize<PasswordConfig>(json);
        }
        
        return new PasswordConfig
        {
            IncludeNumbers = true,
            IncludeUppercase = true,
            PasswordLength = 16
        };
    }
    
    private string GenerateMemorablePassword(PasswordConfig config)
    {
        StringBuilder password = new StringBuilder();
        
        // Select random words
        string adjective = adjectives[random.Next(adjectives.Length)];
        string noun = nouns[random.Next(nouns.Length)];
        string separator = separators[random.Next(separators.Length)];
        
        // Apply transformations
        if (config.IncludeUppercase)
        {
            adjective = CapitalizeFirstLetter(adjective);
            noun = CapitalizeFirstLetter(noun);
        }
        
        password.Append(adjective);
        password.Append(separator);
        password.Append(noun);
        
        if (config.IncludeNumbers)
        {
            password.Append(random.Next(10, 100));
        }
        
        // Ensure password meets length requirement
        while (password.Length < config.PasswordLength)
        {
            string extraWord = random.Next(2) == 0 ? 
                adjectives[random.Next(adjectives.Length)] : 
                nouns[random.Next(nouns.Length)];
                
            if (config.IncludeUppercase)
            {
                extraWord = CapitalizeFirstLetter(extraWord);
            }
            
            password.Append(separators[random.Next(separators.Length)]);
            password.Append(extraWord);
        }
        
        // Trim if too long
        if (password.Length > config.PasswordLength)
        {
            return password.ToString().Substring(0, config.PasswordLength);
        }
        
        return password.ToString();
    }
    
    private string CapitalizeFirstLetter(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;
            
        return char.ToUpper(word[0]) + word.Substring(1);
    }
    
    private void SavePasswordHistory(string dataFolder, string password)
    {
        string historyPath = Path.Combine(dataFolder, "password_history.json");
        PasswordHistory history;
        
        if (File.Exists(historyPath))
        {
            string json = File.ReadAllText(historyPath);
            history = JsonSerializer.Deserialize<PasswordHistory>(json);
        }
        else
        {
            history = new PasswordHistory();
        }
        
        history.Passwords.Add(new PasswordEntry
        {
            Password = password,
            GeneratedDate = DateTime.Now
        });
        
        string updatedJson = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(historyPath, updatedJson);
    }
}

public class PasswordConfig
{
    public bool IncludeNumbers { get; set; }
    public bool IncludeUppercase { get; set; }
    public int PasswordLength { get; set; }
}

public class PasswordHistory
{
    public List<PasswordEntry> Passwords { get; set; } = new List<PasswordEntry>();
}

public class PasswordEntry
{
    public string Password { get; set; }
    public DateTime GeneratedDate { get; set; }
}