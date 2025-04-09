using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class WineCheesePairingJournal : IGeneratedModule
{
    public string Name { get; set; } = "Wine and Cheese Pairing Journal";

    private string _journalFilePath;
    private List<PairingEntry> _pairings;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Wine and Cheese Pairing Journal...");
        
        try
        {
            Directory.CreateDirectory(dataFolder);
            _journalFilePath = Path.Combine(dataFolder, "wine_cheese_pairings.json");
            
            LoadPairings();
            
            bool continueRunning = true;
            while (continueRunning)
            {
                DisplayMenu();
                var choice = Console.ReadLine();
                
                switch (choice)
                {
                    case "1":
                        AddPairing();
                        break;
                    case "2":
                        ViewPairings();
                        break;
                    case "3":
                        SearchPairings();
                        break;
                    case "4":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            
            SavePairings();
            Console.WriteLine("Wine and Cheese Pairing Journal saved successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void LoadPairings()
    {
        if (File.Exists(_journalFilePath))
        {
            var json = File.ReadAllText(_journalFilePath);
            _pairings = JsonSerializer.Deserialize<List<PairingEntry>>(json) ?? new List<PairingEntry>();
        }
        else
        {
            _pairings = new List<PairingEntry>();
        }
    }

    private void SavePairings()
    {
        var json = JsonSerializer.Serialize(_pairings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_journalFilePath, json);
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nWine and Cheese Pairing Journal");
        Console.WriteLine("1. Add new pairing");
        Console.WriteLine("2. View all pairings");
        Console.WriteLine("3. Search pairings");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
    }

    private void AddPairing()
    {
        Console.Write("Enter wine name: ");
        var wine = Console.ReadLine();
        
        Console.Write("Enter cheese name: ");
        var cheese = Console.ReadLine();
        
        Console.Write("Enter tasting notes: ");
        var notes = Console.ReadLine();
        
        Console.Write("Enter rating (1-5): ");
        if (int.TryParse(Console.ReadLine(), out int rating) && rating >= 1 && rating <= 5)
        {
            _pairings.Add(new PairingEntry
            {
                Wine = wine,
                Cheese = cheese,
                Notes = notes,
                Rating = rating,
                DateAdded = DateTime.Now
            });
            
            Console.WriteLine("Pairing added successfully!");
        }
        else
        {
            Console.WriteLine("Invalid rating. Please enter a number between 1 and 5.");
        }
    }

    private void ViewPairings()
    {
        if (_pairings.Count == 0)
        {
            Console.WriteLine("No pairings found.");
            return;
        }
        
        Console.WriteLine("\nAll Wine and Cheese Pairings:");
        foreach (var pairing in _pairings)
        {
            DisplayPairing(pairing);
        }
    }

    private void SearchPairings()
    {
        Console.Write("Enter search term (wine or cheese name): ");
        var term = Console.ReadLine()?.ToLower();
        
        var results = _pairings.FindAll(p => 
            p.Wine.ToLower().Contains(term) || 
            p.Cheese.ToLower().Contains(term));
            
        if (results.Count == 0)
        {
            Console.WriteLine("No matching pairings found.");
        }
        else
        {
            Console.WriteLine("\nMatching Pairings:");
            foreach (var pairing in results)
            {
                DisplayPairing(pairing);
            }
        }
    }

    private void DisplayPairing(PairingEntry pairing)
    {
        Console.WriteLine($"Wine: {pairing.Wine}");
        Console.WriteLine($"Cheese: {pairing.Cheese}");
        Console.WriteLine($"Rating: {new string('★', pairing.Rating)}{new string('☆', 5 - pairing.Rating)}");
        Console.WriteLine($"Notes: {pairing.Notes}");
        Console.WriteLine($"Date Added: {pairing.DateAdded:yyyy-MM-dd}");
        Console.WriteLine();
    }

    private class PairingEntry
    {
        public string Wine { get; set; }
        public string Cheese { get; set; }
        public string Notes { get; set; }
        public int Rating { get; set; }
        public DateTime DateAdded { get; set; }
    }
}