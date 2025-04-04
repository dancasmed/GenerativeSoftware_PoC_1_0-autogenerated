using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class VirtualPetModule
{
    public string Name { get; set; } = "Virtual Pet Simulator";
    
    private string _petDataPath;
    private VirtualPet _pet;
    
    public VirtualPetModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        _petDataPath = Path.Combine(dataFolder, "pet_data.json");
        
        Console.WriteLine("Virtual Pet Simulator is running...");
        
        LoadPet();
        
        if (_pet == null)
        {
            Console.WriteLine("No pet found. Creating a new pet.");
            _pet = new VirtualPet
            {
                Name = "Fluffy",
                Hunger = 50,
                Happiness = 50,
                Energy = 50,
                LastUpdated = DateTime.Now
            };
            SavePet();
        }
        
        UpdatePetStats();
        
        DisplayPetStatus();
        
        return true;
    }
    
    private void LoadPet()
    {
        try
        {
            if (File.Exists(_petDataPath))
            {
                string json = File.ReadAllText(_petDataPath);
                _pet = JsonSerializer.Deserialize<VirtualPet>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading pet data: " + ex.Message);
        }
    }
    
    private void SavePet()
    {
        try
        {
            string json = JsonSerializer.Serialize(_pet);
            File.WriteAllText(_petDataPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving pet data: " + ex.Message);
        }
    }
    
    private void UpdatePetStats()
    {
        TimeSpan timeSinceLastUpdate = DateTime.Now - _pet.LastUpdated;
        int hoursPassed = (int)timeSinceLastUpdate.TotalHours;
        
        if (hoursPassed > 0)
        {
            _pet.Hunger = Math.Min(100, _pet.Hunger + (hoursPassed * 5));
            _pet.Happiness = Math.Max(0, _pet.Happiness - (hoursPassed * 5));
            _pet.Energy = Math.Max(0, _pet.Energy - (hoursPassed * 5));
            _pet.LastUpdated = DateTime.Now;
            SavePet();
        }
    }
    
    private void DisplayPetStatus()
    {
        Console.WriteLine("\n--- Pet Status ---");
        Console.WriteLine("Name: " + _pet.Name);
        Console.WriteLine("Hunger: " + _pet.Hunger + "/100");
        Console.WriteLine("Happiness: " + _pet.Happiness + "/100");
        Console.WriteLine("Energy: " + _pet.Energy + "/100");
        Console.WriteLine("Last updated: " + _pet.LastUpdated);
        
        if (_pet.Hunger >= 80)
        {
            Console.WriteLine("Warning: Your pet is very hungry!");
        }
        
        if (_pet.Happiness <= 20)
        {
            Console.WriteLine("Warning: Your pet is very unhappy!");
        }
        
        if (_pet.Energy <= 20)
        {
            Console.WriteLine("Warning: Your pet is very tired!");
        }
    }
}

public class VirtualPet
{
    public string Name { get; set; }
    public int Hunger { get; set; }
    public int Happiness { get; set; }
    public int Energy { get; set; }
    public DateTime LastUpdated { get; set; }
}