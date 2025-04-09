using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class ATMSimulator : IGeneratedModule
{
    public string Name { get; set; } = "ATM Simulator";
    
    private string _accountFilePath;
    private Account _account;
    
    public bool Main(string dataFolder)
    {
        _accountFilePath = Path.Combine(dataFolder, "account.json");
        
        Console.WriteLine("ATM Simulator is running.");
        
        LoadAccount();
        
        bool exit = false;
        
        while (!exit)
        {
            Console.WriteLine("\nPlease select an option:");
            Console.WriteLine("1. Check Balance");
            Console.WriteLine("2. Deposit Money");
            Console.WriteLine("3. Withdraw Money");
            Console.WriteLine("4. Exit");
            
            string input = Console.ReadLine();
            
            switch (input)
            {
                case "1":
                    CheckBalance();
                    break;
                case "2":
                    DepositMoney();
                    break;
                case "3":
                    WithdrawMoney();
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
        
        SaveAccount();
        Console.WriteLine("Thank you for using the ATM Simulator.");
        
        return true;
    }
    
    private void LoadAccount()
    {
        if (File.Exists(_accountFilePath))
        {
            string json = File.ReadAllText(_accountFilePath);
            _account = JsonSerializer.Deserialize<Account>(json);
        }
        else
        {
            _account = new Account { Balance = 0 };
        }
    }
    
    private void SaveAccount()
    {
        string json = JsonSerializer.Serialize(_account);
        File.WriteAllText(_accountFilePath, json);
    }
    
    private void CheckBalance()
    {
        Console.WriteLine("Your current balance is: " + _account.Balance);
    }
    
    private void DepositMoney()
    {
        Console.WriteLine("Enter amount to deposit:");
        string input = Console.ReadLine();
        
        if (decimal.TryParse(input, out decimal amount) && amount > 0)
        {
            _account.Balance += amount;
            Console.WriteLine("Deposit successful. New balance is: " + _account.Balance);
        }
        else
        {
            Console.WriteLine("Invalid amount. Please enter a positive number.");
        }
    }
    
    private void WithdrawMoney()
    {
        Console.WriteLine("Enter amount to withdraw:");
        string input = Console.ReadLine();
        
        if (decimal.TryParse(input, out decimal amount) && amount > 0)
        {
            if (amount <= _account.Balance)
            {
                _account.Balance -= amount;
                Console.WriteLine("Withdrawal successful. New balance is: " + _account.Balance);
            }
            else
            {
                Console.WriteLine("Insufficient funds.");
            }
        }
        else
        {
            Console.WriteLine("Invalid amount. Please enter a positive number.");
        }
    }
}

public class Account
{
    public decimal Balance { get; set; }
}