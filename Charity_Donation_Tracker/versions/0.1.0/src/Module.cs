using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CharityDonationTracker : IGeneratedModule
{
    public string Name { get; set; } = "Charity Donation Tracker";

    private string donationsFilePath;
    
    public CharityDonationTracker()
    {
    }

    public bool Main(string dataFolder)
    {
        donationsFilePath = Path.Combine(dataFolder, "donations.json");
        
        Console.WriteLine("Charity Donation Tracker module is running.");
        Console.WriteLine("Data will be saved to: " + donationsFilePath);
        
        bool continueRunning = true;
        while (continueRunning)
        {
            DisplayMenu();
            
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddDonation();
                    break;
                case "2":
                    ViewAllDonations();
                    break;
                case "3":
                    CalculateTotalDonations();
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Charity Donation Tracker module completed.");
        return true;
    }
    
    private void DisplayMenu()
    {
        Console.WriteLine("\nCharity Donation Tracker");
        Console.WriteLine("1. Add a donation");
        Console.WriteLine("2. View all donations");
        Console.WriteLine("3. Calculate total donations");
        Console.WriteLine("4. Exit");
        Console.Write("Select an option: ");
    }
    
    private void AddDonation()
    {
        Console.Write("Enter donor name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter donation amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }
        
        Console.Write("Enter donation date (YYYY-MM-DD): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            Console.WriteLine("Invalid date format. Using current date.");
            date = DateTime.Now;
        }
        
        var donation = new Donation
        {
            DonorName = name,
            Amount = amount,
            Date = date
        };
        
        List<Donation> donations = LoadDonations();
        donations.Add(donation);
        SaveDonations(donations);
        
        Console.WriteLine("Donation added successfully.");
    }
    
    private void ViewAllDonations()
    {
        List<Donation> donations = LoadDonations();
        
        if (donations.Count == 0)
        {
            Console.WriteLine("No donations recorded yet.");
            return;
        }
        
        Console.WriteLine("\nAll Donations:");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("{0,-20} {1,10} {2,15}", "Donor Name", "Amount", "Date");
        Console.WriteLine("--------------------------------------------------");
        
        foreach (var donation in donations)
        {
            Console.WriteLine("{0,-20} {1,10:C2} {2,15:d}", 
                donation.DonorName, 
                donation.Amount, 
                donation.Date);
        }
    }
    
    private void CalculateTotalDonations()
    {
        List<Donation> donations = LoadDonations();
        decimal total = 0;
        
        foreach (var donation in donations)
        {
            total += donation.Amount;
        }
        
        Console.WriteLine("Total donations: " + total.ToString("C2"));
    }
    
    private List<Donation> LoadDonations()
    {
        if (!File.Exists(donationsFilePath))
        {
            return new List<Donation>();
        }
        
        string json = File.ReadAllText(donationsFilePath);
        return JsonSerializer.Deserialize<List<Donation>>(json) ?? new List<Donation>();
    }
    
    private void SaveDonations(List<Donation> donations)
    {
        string json = JsonSerializer.Serialize(donations);
        File.WriteAllText(donationsFilePath, json);
    }
}

public class Donation
{
    public string DonorName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}