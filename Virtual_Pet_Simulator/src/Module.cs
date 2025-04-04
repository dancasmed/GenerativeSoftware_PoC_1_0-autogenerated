using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class VirtualPetModule : IGeneratedModule
{
    public string Name { get; set; } = "Virtual Pet Simulator";
    
    private string petDataPath;
    private PetData petData;
    
    public bool Main(string dataFolder)
    {
        petDataPath = Path.Combine(dataFolder, "petData.json");
        
        Console.WriteLine("Virtual Pet Simulator is running...");
        
        LoadPetData();
        
        bool running = true;
        while (running)
        {
            DisplayStatus();
            
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Feed pet");
            Console.WriteLine("2. Play with pet");
            Console.WriteLine("3. Let pet rest");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");
            
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                switch (choice)
                {
                    case 1:
                        FeedPet();
                        break;
                    case 2:
                        PlayWithPet();
                        break;
                    case 3:
                        RestPet();
                        break;
                    case 4:
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
                
                UpdatePetStatus();
                SavePetData();
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a number.");
            }
        }
        
        Console.WriteLine("Saving pet data and exiting...");
        return true;
    }
    
    private void LoadPetData()
    {
        try
        {
            if (File.Exists(petDataPath))
            {
                string json = File.ReadAllText(petDataPath);
                petData = JsonSerializer.Deserialize<PetData>(json);
                Console.WriteLine("Pet data loaded successfully.");
            }
            else
            {
                petData = new PetData
                {
                    Name = "Fluffy",
                    Hunger = 50,
                    Happiness = 50,
                    Energy = 50,
                    LastUpdated = DateTime.Now
                };
                Console.WriteLine("Created a new pet.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading pet data: " + ex.Message);
            petData = new PetData
            {
                Name = "Fluffy",
                Hunger = 50,
                Happiness = 50,
                Energy = 50,
                LastUpdated = DateTime.Now
            };
        }
    }
    
    private void SavePetData()
    {
        try
        {
            string json = JsonSerializer.Serialize(petData);
            File.WriteAllText(petDataPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving pet data: " + ex.Message);
        }
    }
    
    private void DisplayStatus()
    {
        Console.WriteLine("\n--- Pet Status ---");
        Console.WriteLine("Name: " + petData.Name);
        Console.WriteLine("Hunger: " + petData.Hunger + "/100");
        Console.WriteLine("Happiness: " + petData.Happiness + "/100");
        Console.WriteLine("Energy: " + petData.Energy + "/100");
    }
    
    private void FeedPet()
    {
        petData.Hunger = Math.Min(100, petData.Hunger + 20);
        petData.Energy = Math.Min(100, petData.Energy + 5);
        Console.WriteLine("You fed " + petData.Name + ". Hunger decreased and energy slightly increased.");
    }
    
    private void PlayWithPet()
    {
        petData.Happiness = Math.Min(100, petData.Happiness + 20);
        petData.Energy = Math.Max(0, petData.Energy - 15);
        petData.Hunger = Math.Min(100, petData.Hunger + 10);
        Console.WriteLine("You played with " + petData.Name + ". Happiness increased but energy decreased.");
    }
    
    private void RestPet()
    {
        petData.Energy = Math.Min(100, petData.Energy + 30);
        petData.Happiness = Math.Max(0, petData.Happiness - 5);
        petData.Hunger = Math.Min(100, petData.Hunger + 5);
        Console.WriteLine(petData.Name + " is resting. Energy restored but happiness slightly decreased.");
    }
    
    private void UpdatePetStatus()
    {
        TimeSpan timePassed = DateTime.Now - petData.LastUpdated;
        
        // Degrade stats over time
        int minutesPassed = (int)timePassed.TotalMinutes;
        if (minutesPassed > 0)
        {
            petData.Hunger = Math.Min(100, petData.Hunger + (minutesPassed * 2));
            petData.Happiness = Math.Max(0, petData.Happiness - minutesPassed);
            petData.Energy = Math.Max(0, petData.Energy - minutesPassed);
            petData.LastUpdated = DateTime.Now;
        }
    }
}

public class PetData
{
    public string Name { get; set; }
    public int Hunger { get; set; }
    public int Happiness { get; set; }
    public int Energy { get; set; }
    public DateTime LastUpdated { get; set; }
}