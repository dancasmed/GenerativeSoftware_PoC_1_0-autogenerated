using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CharityDonationTracker : IGeneratedModule
{
    public string Name { get; set; } = "Charity Donation Tracker";

    private string donationsFilePath;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Charity Donation Tracker module...");

        donationsFilePath = Path.Combine(dataFolder, "donations.json");

        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        bool running = true;
        while (running)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add Donation");
            Console.WriteLine("2. View All Donations");
            Console.WriteLine("3. Calculate Total Donations");
            Console.WriteLine("4. Exit");
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
                    CalculateTotalDonations();
                    break;
                case "4":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        Console.WriteLine("Exiting Charity Donation Tracker module...");
        return true;
    }

    private void AddDonation()
    {
        Console.Write("Enter donor name: ");
        string name = Console.ReadLine();

        Console.Write("Enter donor email: ");
        string email = Console.ReadLine();

        Console.Write("Enter donation amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }

        var donation = new Donation
        {
            DonorName = name,
            DonorEmail = email,
            Amount = amount,
            Date = DateTime.Now
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
        foreach (var donation in donations)
        {
            Console.WriteLine($"Donor: {donation.DonorName}");
            Console.WriteLine($"Email: {donation.DonorEmail}");
            Console.WriteLine($"Amount: {donation.Amount:C}");
            Console.WriteLine($"Date: {donation.Date}");
            Console.WriteLine("----------------------------");
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

        Console.WriteLine($"Total donations: {total:C}");
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

    private class Donation
    {
        public string DonorName { get; set; }
        public string DonorEmail { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}