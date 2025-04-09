using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Threading;

public class SoundFrequencyGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Sound Frequency Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Sound Frequency Generator Module is running.");
        Console.WriteLine("This module generates sound frequencies for testing purposes.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "sound_config.json");
            
            if (!File.Exists(configPath))
            {
                var defaultConfig = new SoundConfig
                {
                    Frequency = 440,
                    Duration = 1000
                };
                
                string json = System.Text.Json.JsonSerializer.Serialize(defaultConfig);
                File.WriteAllText(configPath, json);
                Console.WriteLine("Default configuration file created at: " + configPath);
            }
            
            string jsonConfig = File.ReadAllText(configPath);
            var config = System.Text.Json.JsonSerializer.Deserialize<SoundConfig>(jsonConfig);
            
            Console.WriteLine("Generating sound with frequency: " + config.Frequency + " Hz for " + config.Duration + " ms");
            
            GenerateSound(config.Frequency, config.Duration);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private void GenerateSound(int frequency, int duration)
    {
        if (frequency <= 0)
        {
            throw new ArgumentException("Frequency must be greater than 0");
        }
        
        if (duration <= 0)
        {
            throw new ArgumentException("Duration must be greater than 0");
        }
        
        try
        {
            Console.Beep(frequency, duration);
        }
        catch (PlatformNotSupportedException)
        {
            Console.WriteLine("Console.Beep is not supported on this platform.");
            Console.WriteLine("Would have played frequency: " + frequency + " Hz for " + duration + " ms");
        }
    }
}

public class SoundConfig
{
    public int Frequency { get; set; }
    public int Duration { get; set; }
}