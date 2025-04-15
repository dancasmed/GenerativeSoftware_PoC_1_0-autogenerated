using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MedsManagerModule
{
    public string Name { get; set; } = "Medication Manager";
    
    private string _medsFilePath;
    
    public MedsManagerModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Medication Manager Module is running...");
        
        _medsFilePath = Path.Combine(dataFolder, "medications.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    AddMedication();
                    break;
                case "2":
                    ViewMedications();
                    break;
                case "3":
                    TakeMedication();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Medication Manager is shutting down...");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nMedication Manager");
        Console.WriteLine("1. Add Medication");
        Console.WriteLine("2. View Medications");
        Console.WriteLine("3. Take Medication");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddMedication()
    {
        Console.Write("Enter medication name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter dosage (e.g., 50mg): ");
        string dosage = Console.ReadLine();
        
        Console.Write("Enter frequency (e.g., 3 times daily): ");
        string frequency = Console.ReadLine();
        
        var medications = LoadMedications();
        medications.Add(new Medication
        {
            Id = Guid.NewGuid(),
            Name = name,
            Dosage = dosage,
            Frequency = frequency,
            LastTaken = null,
            TimesTaken = 0
        });
        
        SaveMedications(medications);
        Console.WriteLine("Medication added successfully!");
    }
    
    private void ViewMedications()
    {
        var medications = LoadMedications();
        
        if (medications.Count == 0)
        {
            Console.WriteLine("No medications found.");
            return;
        }
        
        Console.WriteLine("\nYour Medications:");
        foreach (var med in medications)
        {
            Console.WriteLine($"{med.Name} - {med.Dosage} - {med.Frequency}");
            Console.WriteLine($"Last taken: {(med.LastTaken.HasValue ? med.LastTaken.Value.ToString("g") : "Never")}");
            Console.WriteLine($"Times taken: {med.TimesTaken}\n");
        }
    }
    
    private void TakeMedication()
    {
        var medications = LoadMedications();
        
        if (medications.Count == 0)
        {
            Console.WriteLine("No medications to take.");
            return;
        }
        
        Console.WriteLine("\nSelect medication to take:");
        for (int i = 0; i < medications.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {medications[i].Name} - {medications[i].Dosage}");
        }
        
        if (int.TryParse(Console.ReadLine(), out int selection) && selection > 0 && selection <= medications.Count)
        {
            var med = medications[selection - 1];
            med.LastTaken = DateTime.Now;
            med.TimesTaken++;
            
            SaveMedications(medications);
            Console.WriteLine($"{med.Name} taken at {med.LastTaken.Value.ToString("g")}");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }
    }
    
    private List<Medication> LoadMedications()
    {
        if (!File.Exists(_medsFilePath))
        {
            return new List<Medication>();
        }
        
        string json = File.ReadAllText(_medsFilePath);
        return JsonSerializer.Deserialize<List<Medication>>(json);
    }
    
    private void SaveMedications(List<Medication> medications)
    {
        string json = JsonSerializer.Serialize(medications);
        File.WriteAllText(_medsFilePath, json);
    }
}

public class Medication
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Dosage { get; set; }
    public string Frequency { get; set; }
    public DateTime? LastTaken { get; set; }
    public int TimesTaken { get; set; }
}