using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class PetTrainingTracker : IGeneratedModule
{
    public string Name { get; set; } = "Pet Training Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Pet Training Tracker module is running.");
        
        _dataFilePath = Path.Combine(dataFolder, "pet_training_data.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        List<Pet> pets = LoadPets();
        
        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nPet Training Tracker");
            Console.WriteLine("1. Add Pet");
            Console.WriteLine("2. Add Command");
            Console.WriteLine("3. View Pets");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddPet(pets);
                    break;
                case "2":
                    AddCommand(pets);
                    break;
                case "3":
                    ViewPets(pets);
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
            
            SavePets(pets);
        }
        
        return true;
    }
    
    private List<Pet> LoadPets()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<List<Pet>>(json);
        }
        
        return new List<Pet>();
    }
    
    private void SavePets(List<Pet> pets)
    {
        string json = JsonSerializer.Serialize(pets);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void AddPet(List<Pet> pets)
    {
        Console.Write("Enter pet name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter pet type (e.g., Dog, Cat): ");
        string type = Console.ReadLine();
        
        pets.Add(new Pet { Name = name, Type = type, Commands = new List<string>() });
        Console.WriteLine("Pet added successfully.");
    }
    
    private void AddCommand(List<Pet> pets)
    {
        if (pets.Count == 0)
        {
            Console.WriteLine("No pets available. Please add a pet first.");
            return;
        }
        
        Console.WriteLine("Select a pet:");
        for (int i = 0; i < pets.Count; i++)
        {
            Console.WriteLine(i + 1 + ". " + pets[i].Name + " (" + pets[i].Type + ")");
        }
        
        Console.Write("Enter pet number: ");
        if (int.TryParse(Console.ReadLine(), out int petIndex) && petIndex > 0 && petIndex <= pets.Count)
        {
            Console.Write("Enter command to add: ");
            string command = Console.ReadLine();
            pets[petIndex - 1].Commands.Add(command);
            Console.WriteLine("Command added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid pet number.");
        }
    }
    
    private void ViewPets(List<Pet> pets)
    {
        if (pets.Count == 0)
        {
            Console.WriteLine("No pets available.");
            return;
        }
        
        foreach (var pet in pets)
        {
            Console.WriteLine("\nName: " + pet.Name);
            Console.WriteLine("Type: " + pet.Type);
            Console.WriteLine("Commands learned:");
            
            if (pet.Commands.Count == 0)
            {
                Console.WriteLine("None");
            }
            else
            {
                foreach (var command in pet.Commands)
                {
                    Console.WriteLine("- " + command);
                }
            }
        }
    }
}

public class Pet
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<string> Commands { get; set; }
}