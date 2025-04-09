using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class CrowdfundingTracker : IGeneratedModule
{
    public string Name { get; set; } = "Crowdfunding Campaign Tracker";
    
    private string _dataFilePath;
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Crowdfunding Campaign Tracker...");
        
        _dataFilePath = Path.Combine(dataFolder, "crowdfunding_data.json");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }
        
        CampaignData campaignData = LoadCampaignData();
        
        bool continueRunning = true;
        while (continueRunning)
        {
            Console.WriteLine("\nCrowdfunding Campaign Tracker");
            Console.WriteLine("1. View Campaign Progress");
            Console.WriteLine("2. Add Donation");
            Console.WriteLine("3. Update Campaign Goal");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    ViewCampaignProgress(campaignData);
                    break;
                case "2":
                    AddDonation(campaignData);
                    SaveCampaignData(campaignData);
                    break;
                case "3":
                    UpdateCampaignGoal(campaignData);
                    SaveCampaignData(campaignData);
                    break;
                case "4":
                    continueRunning = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        Console.WriteLine("Crowdfunding Campaign Tracker has finished.");
        return true;
    }
    
    private CampaignData LoadCampaignData()
    {
        if (File.Exists(_dataFilePath))
        {
            string json = File.ReadAllText(_dataFilePath);
            return JsonSerializer.Deserialize<CampaignData>(json) ?? new CampaignData();
        }
        
        return new CampaignData { GoalAmount = 1000, Donations = new List<Donation>() };
    }
    
    private void SaveCampaignData(CampaignData data)
    {
        string json = JsonSerializer.Serialize(data);
        File.WriteAllText(_dataFilePath, json);
    }
    
    private void ViewCampaignProgress(CampaignData data)
    {
        decimal totalDonated = 0;
        foreach (var donation in data.Donations)
        {
            totalDonated += donation.Amount;
        }
        
        Console.WriteLine("\nCampaign Progress:");
        Console.WriteLine("Goal Amount: " + data.GoalAmount.ToString("C"));
        Console.WriteLine("Total Donated: " + totalDonated.ToString("C"));
        Console.WriteLine("Remaining: " + (data.GoalAmount - totalDonated).ToString("C"));
        Console.WriteLine("Percentage: " + (totalDonated / data.GoalAmount * 100).ToString("F2") + "%");
        
        if (data.Donations.Count > 0)
        {
            Console.WriteLine("\nRecent Donations:");
            foreach (var donation in data.Donations)
            {
                Console.WriteLine(donation.Date.ToString("yyyy-MM-dd") + " - " + donation.DonorName + " - " + donation.Amount.ToString("C"));
            }
        }
    }
    
    private void AddDonation(CampaignData data)
    {
        Console.Write("\nEnter donor name: ");
        string donorName = Console.ReadLine();
        
        Console.Write("Enter donation amount: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            data.Donations.Add(new Donation
            {
                DonorName = donorName,
                Amount = amount,
                Date = DateTime.Now
            });
            
            Console.WriteLine("Donation added successfully.");
        }
        else
        {
            Console.WriteLine("Invalid amount. Donation not added.");
        }
    }
    
    private void UpdateCampaignGoal(CampaignData data)
    {
        Console.Write("\nEnter new campaign goal: ");
        if (decimal.TryParse(Console.ReadLine(), out decimal newGoal) && newGoal > 0)
        {
            data.GoalAmount = newGoal;
            Console.WriteLine("Campaign goal updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid amount. Goal not updated.");
        }
    }
}

public class CampaignData
{
    public decimal GoalAmount { get; set; }
    public List<Donation> Donations { get; set; } = new List<Donation>();
}

public class Donation
{
    public string DonorName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}