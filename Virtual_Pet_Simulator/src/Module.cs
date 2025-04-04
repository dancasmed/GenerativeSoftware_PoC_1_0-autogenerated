using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class VirtualPetModule : IGeneratedModule
{
    public string Name { get; set; } = "Virtual Pet Simulator";

    private string petDataPath;
    private Pet pet;

    public VirtualPetModule()
    {
    }

    public bool Main(string dataFolder)
    {
        petDataPath = Path.Combine(dataFolder, "pet_data.json");
        LoadPetData();

        Console.WriteLine("Virtual Pet Simulator started!");
        Console.WriteLine("Your pet " + pet.Name + " is ready to play.");
        Console.WriteLine("Commands: feed, play, status, exit");

        bool running = true;
        while (running)
        {
            Console.Write("> ");
            string input = Console.ReadLine().ToLower().Trim();

            switch (input)
            {
                case "feed":
                    FeedPet();
                    break;
                case "play":
                    PlayWithPet();
                    break;
                case "status":
                    ShowStatus();
                    break;
                case "exit":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Unknown command. Try: feed, play, status, exit");
                    break;
            }

            UpdatePetState();
            SavePetData();
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
                pet = JsonSerializer.Deserialize<Pet>(json);
                Console.WriteLine("Loaded existing pet data.");
            }
            else
            {
                pet = new Pet
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
            pet = new Pet
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
            string json = JsonSerializer.Serialize(pet);
            File.WriteAllText(petDataPath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving pet data: " + ex.Message);
        }
    }

    private void FeedPet()
    {
        pet.Hunger = Math.Max(0, pet.Hunger - 30);
        pet.Energy = Math.Min(100, pet.Energy + 10);
        Console.WriteLine("You fed " + pet.Name + ". Hunger decreased, energy increased.");
    }

    private void PlayWithPet()
    {
        if (pet.Energy >= 20)
        {
            pet.Happiness = Math.Min(100, pet.Happiness + 25);
            pet.Energy = Math.Max(0, pet.Energy - 20);
            pet.Hunger = Math.Min(100, pet.Hunger + 15);
            Console.WriteLine("You played with " + pet.Name + ". Happiness increased, energy decreased.");
        }
        else
        {
            Console.WriteLine(pet.Name + " is too tired to play right now!");
        }
    }

    private void ShowStatus()
    {
        Console.WriteLine("--- " + pet.Name + "'s Status ---");
        Console.WriteLine("Hunger: " + pet.Hunger + "/100");
        Console.WriteLine("Happiness: " + pet.Happiness + "/100");
        Console.WriteLine("Energy: " + pet.Energy + "/100");
        Console.WriteLine("Last activity: " + (DateTime.Now - pet.LastUpdated).TotalMinutes.ToString("0") + " minutes ago");
    }

    private void UpdatePetState()
    {
        TimeSpan timePassed = DateTime.Now - pet.LastUpdated;
        double minutesPassed = timePassed.TotalMinutes;

        if (minutesPassed > 1)
        {
            // Degrade stats over time
            pet.Hunger = Math.Min(100, pet.Hunger + (int)(minutesPassed * 0.5));
            pet.Happiness = Math.Max(0, pet.Happiness - (int)(minutesPassed * 0.3));
            pet.Energy = Math.Min(100, pet.Energy + (int)(minutesPassed * 0.2));
        }

        pet.LastUpdated = DateTime.Now;
    }
}

public class Pet
{
    public string Name { get; set; }
    public int Hunger { get; set; }
    public int Happiness { get; set; }
    public int Energy { get; set; }
    public DateTime LastUpdated { get; set; }
}