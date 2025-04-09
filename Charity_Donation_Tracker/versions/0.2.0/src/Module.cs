using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CharityDonationTracker : IGeneratedModule
{
    public string Name { get; set; } = "Charity Donation Tracker";
    
    private string _donationsFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Charity Donation Tracker module...");
        
        _donationsFilePath = Path.Combine(dataFolder, "donations.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        bool continueRunning = true;
        
        while (continueRunning)
        {
            Console.WriteLine("\nCharity Donation Tracker");
            Console.WriteLine("1. Add Donation");
            Console.WriteLine("2. View All Donations");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
            
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
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Charity Donation Tracker module finished.");
        return true;
    }
    
    private void AddDonation()
    {
        Console.Write("Enter donor name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter donation amount: ");
        string amountInput = Console.ReadLine();
        
        if (!decimal.TryParse(amountInput, out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }
        
        Console.Write("Enter donation date (YYYY-MM-DD): ");
        string dateInput = Console.ReadLine();
        
        if (!DateTime.TryParse(dateInput, out DateTime date))
        {
            Console.WriteLine("Invalid date. Please use YYYY-MM-DD format.");
            return;
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
        
        Console.WriteLine("Donation added successfully!");
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
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine("Donor Name\tAmount\tDate");
        Console.WriteLine("-------------------------------------------------");
        
        foreach (var donation in donations)
        {
            Console.WriteLine(string.Format("{0}\t{1:C}\t{2:yyyy-MM-dd}", 
                donation.DonorName, donation.Amount, donation.Date));
        }
        
        decimal total = 0;
        foreach (var donation in donations)
        {
            total += donation.Amount;
        }
        
        Console.WriteLine("-------------------------------------------------");
        Console.WriteLine(string.Format("Total: {0:C}", total));
    }
    
    private List<Donation> LoadDonations()
    {
        if (!File.Exists(_donationsFilePath))
        {
            return new List<Donation>();
        }
        
        string json = File.ReadAllText(_donationsFilePath);
        return JsonSerializer.Deserialize<List<Donation>>(json) ?? new List<Donation>();
    }
    
    private void SaveDonations(List<Donation> donations)
    {
        string json = JsonSerializer.Serialize(donations);
        File.WriteAllText(_donationsFilePath, json);
    }
}

public class Donation
{
    public string DonorName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}