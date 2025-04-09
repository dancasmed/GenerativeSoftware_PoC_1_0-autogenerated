using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class SubscriptionTracker : IGeneratedModule
{
    public string Name { get; set; } = "Subscription Tracker";

    private string _dataFilePath;
    private List<Subscription> _subscriptions;

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Subscription Tracker module is running...");
        
        _dataFilePath = Path.Combine(dataFolder, "subscriptions.json");
        LoadSubscriptions();
        
        CheckDuePayments();
        
        return true;
    }

    private void LoadSubscriptions()
    {
        try
        {
            if (File.Exists(_dataFilePath))
            {
                string json = File.ReadAllText(_dataFilePath);
                _subscriptions = JsonSerializer.Deserialize<List<Subscription>>(json);
            }
            else
            {
                _subscriptions = new List<Subscription>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading subscriptions: " + ex.Message);
            _subscriptions = new List<Subscription>();
        }
    }

    private void SaveSubscriptions()
    {
        try
        {
            string json = JsonSerializer.Serialize(_subscriptions);
            File.WriteAllText(_dataFilePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving subscriptions: " + ex.Message);
        }
    }

    private void CheckDuePayments()
    {
        DateTime today = DateTime.Today;
        bool anyDue = false;
        
        foreach (var sub in _subscriptions)
        {
            if (sub.NextPaymentDate <= today)
            {
                Console.WriteLine("ALERT: Payment due for '" + sub.Name + "' on " + sub.NextPaymentDate.ToString("yyyy-MM-dd") + ". Amount: " + sub.Amount.ToString("C"));
                anyDue = true;
                
                // Update next payment date
                sub.NextPaymentDate = sub.NextPaymentDate.AddMonths(sub.BillingCycleMonths);
            }
        }
        
        if (anyDue)
        {
            SaveSubscriptions();
        }
        else
        {
            Console.WriteLine("No subscription payments due at this time.");
        }
    }
}

public class Subscription
{
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public DateTime NextPaymentDate { get; set; }
    public int BillingCycleMonths { get; set; }
}