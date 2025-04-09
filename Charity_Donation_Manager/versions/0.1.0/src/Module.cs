using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CharityDonationManager : IGeneratedModule
{
    public string Name { get; set; } = "Charity Donation Manager";
    
    private string donationsFilePath;
    private string eventsFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Initializing Charity Donation Manager...");
        
        donationsFilePath = Path.Combine(dataFolder, "donations.json");
        eventsFilePath = Path.Combine(dataFolder, "events.json");
        
        EnsureDataFilesExist();
        
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nCharity Donation Manager");
            Console.WriteLine("1. Record Donation");
            Console.WriteLine("2. List All Donations");
            Console.WriteLine("3. Create Fundraising Event");
            Console.WriteLine("4. List All Events");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    RecordDonation();
                    break;
                case "2":
                    ListDonations();
                    break;
                case "3":
                    CreateEvent();
                    break;
                case "4":
                    ListEvents();
                    break;
                case "5":
                    running = false;
                    Console.WriteLine("Saving data and exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        return true;
    }
    
    private void EnsureDataFilesExist()
    {
        if (!File.Exists(donationsFilePath))
        {
            File.WriteAllText(donationsFilePath, "[]");
        }
        
        if (!File.Exists(eventsFilePath))
        {
            File.WriteAllText(eventsFilePath, "[]");
        }
    }
    
    private void RecordDonation()
    {
        Console.Write("Enter donor name: ");
        string donorName = Console.ReadLine();
        
        Console.Write("Enter donation amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Invalid amount. Please enter a valid number.");
            return;
        }
        
        Console.Write("Enter donation date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime date))
        {
            date = DateTime.Now;
        }
        
        var donation = new Donation
        {
            Id = Guid.NewGuid(),
            DonorName = donorName,
            Amount = amount,
            Date = date
        };
        
        var donations = LoadDonations();
        donations.Add(donation);
        SaveDonations(donations);
        
        Console.WriteLine("Donation recorded successfully!");
    }
    
    private void ListDonations()
    {
        var donations = LoadDonations();
        
        Console.WriteLine("\nAll Donations:");
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine("ID\t\t\tDonor\t\tAmount\t\tDate");
        Console.WriteLine("--------------------------------------------------");
        
        foreach (var donation in donations)
        {
            Console.WriteLine(string.Format("{0}\t{1}\t{2:C}\t{3:yyyy-MM-dd}", 
                donation.Id, 
                donation.DonorName, 
                donation.Amount, 
                donation.Date));
        }
        
        Console.WriteLine("--------------------------------------------------");
        Console.WriteLine(string.Format("Total donations: {0:C}", CalculateTotalDonations()));
    }
    
    private void CreateEvent()
    {
        Console.Write("Enter event name: ");
        string eventName = Console.ReadLine();
        
        Console.Write("Enter event date (yyyy-MM-dd): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime eventDate))
        {
            eventDate = DateTime.Now.AddDays(7);
        }
        
        Console.Write("Enter fundraising goal: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal goal))
        {
            Console.WriteLine("Invalid goal amount. Please enter a valid number.");
            return;
        }
        
        var fundraisingEvent = new FundraisingEvent
        {
            Id = Guid.NewGuid(),
            Name = eventName,
            Date = eventDate,
            FundraisingGoal = goal,
            Donations = new List<Guid>()
        };
        
        var events = LoadEvents();
        events.Add(fundraisingEvent);
        SaveEvents(events);
        
        Console.WriteLine("Fundraising event created successfully!");
    }
    
    private void ListEvents()
    {
        var events = LoadEvents();
        var donations = LoadDonations();
        
        Console.WriteLine("\nAll Fundraising Events:");
        Console.WriteLine("----------------------------------------------------------------------------");
        Console.WriteLine("ID\t\t\tEvent Name\t\tDate\t\tGoal\t\tRaised");
        Console.WriteLine("----------------------------------------------------------------------------");
        
        foreach (var ev in events)
        {
            decimal raised = 0;
            foreach (var donationId in ev.Donations)
            {
                var donation = donations.Find(d => d.Id == donationId);
                if (donation != null)
                {
                    raised += donation.Amount;
                }
            }
            
            Console.WriteLine(string.Format("{0}\t{1}\t{2:yyyy-MM-dd}\t{3:C}\t{4:C}", 
                ev.Id, 
                ev.Name, 
                ev.Date, 
                ev.FundraisingGoal, 
                raised));
        }
        
        Console.WriteLine("----------------------------------------------------------------------------");
    }
    
    private List<Donation> LoadDonations()
    {
        string json = File.ReadAllText(donationsFilePath);
        return JsonSerializer.Deserialize<List<Donation>>(json) ?? new List<Donation>();
    }
    
    private void SaveDonations(List<Donation> donations)
    {
        string json = JsonSerializer.Serialize(donations);
        File.WriteAllText(donationsFilePath, json);
    }
    
    private List<FundraisingEvent> LoadEvents()
    {
        string json = File.ReadAllText(eventsFilePath);
        return JsonSerializer.Deserialize<List<FundraisingEvent>>(json) ?? new List<FundraisingEvent>();
    }
    
    private void SaveEvents(List<FundraisingEvent> events)
    {
        string json = JsonSerializer.Serialize(events);
        File.WriteAllText(eventsFilePath, json);
    }
    
    private decimal CalculateTotalDonations()
    {
        var donations = LoadDonations();
        decimal total = 0;
        
        foreach (var donation in donations)
        {
            total += donation.Amount;
        }
        
        return total;
    }
}

public class Donation
{
    public Guid Id { get; set; }
    public string DonorName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}

public class FundraisingEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public decimal FundraisingGoal { get; set; }
    public List<Guid> Donations { get; set; }
}