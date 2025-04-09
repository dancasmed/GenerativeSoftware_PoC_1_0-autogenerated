using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class LoanManager : IGeneratedModule
{
    public string Name { get; set; } = "Loan Manager";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Loan Manager module is running.");
        
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        string loansFilePath = Path.Combine(dataFolder, "loans.json");
        List<Loan> loans = LoadLoans(loansFilePath);

        bool exit = false;
        while (!exit)
        {
            Console.WriteLine("\nOptions:");
            Console.WriteLine("1. Add a new loan");
            Console.WriteLine("2. View all loans");
            Console.WriteLine("3. Generate payment schedule");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddLoan(loans);
                    break;
                case "2":
                    ViewLoans(loans);
                    break;
                case "3":
                    GeneratePaymentSchedule(loans);
                    break;
                case "4":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            SaveLoans(loans, loansFilePath);
        }

        return true;
    }

    private List<Loan> LoadLoans(string filePath)
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<List<Loan>>(json) ?? new List<Loan>();
        }
        return new List<Loan>();
    }

    private void SaveLoans(List<Loan> loans, string filePath)
    {
        string json = JsonSerializer.Serialize(loans);
        File.WriteAllText(filePath, json);
    }

    private void AddLoan(List<Loan> loans)
    {
        Console.Write("Enter borrower name: ");
        string borrowerName = Console.ReadLine();

        Console.Write("Enter loan amount: ");
        decimal amount = decimal.Parse(Console.ReadLine());

        Console.Write("Enter annual interest rate (%): ");
        double interestRate = double.Parse(Console.ReadLine());

        Console.Write("Enter loan term in months: ");
        int termMonths = int.Parse(Console.ReadLine());

        loans.Add(new Loan
        {
            Id = Guid.NewGuid(),
            BorrowerName = borrowerName,
            Amount = amount,
            AnnualInterestRate = interestRate,
            TermMonths = termMonths,
            StartDate = DateTime.Now
        });

        Console.WriteLine("Loan added successfully.");
    }

    private void ViewLoans(List<Loan> loans)
    {
        if (loans.Count == 0)
        {
            Console.WriteLine("No loans found.");
            return;
        }

        foreach (var loan in loans)
        {
            Console.WriteLine("\nLoan ID: " + loan.Id);
            Console.WriteLine("Borrower: " + loan.BorrowerName);
            Console.WriteLine("Amount: " + loan.Amount.ToString("C"));
            Console.WriteLine("Interest Rate: " + loan.AnnualInterestRate + "%");
            Console.WriteLine("Term: " + loan.TermMonths + " months");
            Console.WriteLine("Start Date: " + loan.StartDate.ToShortDateString());
        }
    }

    private void GeneratePaymentSchedule(List<Loan> loans)
    {
        if (loans.Count == 0)
        {
            Console.WriteLine("No loans found.");
            return;
        }

        ViewLoans(loans);
        Console.Write("\nEnter the Loan ID to generate schedule: ");
        string loanIdInput = Console.ReadLine();

        if (Guid.TryParse(loanIdInput, out Guid loanId))
        {
            Loan selectedLoan = loans.Find(l => l.Id == loanId);
            if (selectedLoan != null)
            {
                Console.WriteLine("\nPayment Schedule for " + selectedLoan.BorrowerName);
                Console.WriteLine("Loan Amount: " + selectedLoan.Amount.ToString("C"));
                Console.WriteLine("Annual Interest Rate: " + selectedLoan.AnnualInterestRate + "%");
                Console.WriteLine("Term: " + selectedLoan.TermMonths + " months");
                Console.WriteLine("\nPayment #\tDate\t\tPayment\t\tPrincipal\tInterest\tRemaining Balance");

                decimal monthlyRate = (decimal)(selectedLoan.AnnualInterestRate / 100 / 12);
                decimal monthlyPayment = selectedLoan.Amount * monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, selectedLoan.TermMonths) / 
                                        (decimal)(Math.Pow(1 + (double)monthlyRate, selectedLoan.TermMonths) - 1);
                decimal remainingBalance = selectedLoan.Amount;

                for (int i = 1; i <= selectedLoan.TermMonths; i++)
                {
                    DateTime paymentDate = selectedLoan.StartDate.AddMonths(i);
                    decimal interestPayment = remainingBalance * monthlyRate;
                    decimal principalPayment = monthlyPayment - interestPayment;
                    remainingBalance -= principalPayment;

                    if (i == selectedLoan.TermMonths)
                    {
                        principalPayment += remainingBalance;
                        monthlyPayment += remainingBalance;
                        remainingBalance = 0;
                    }

                    Console.WriteLine($"{i}\t\t{paymentDate.ToShortDateString()}\t{monthlyPayment.ToString("C")}\t\t{principalPayment.ToString("C")}\t\t{interestPayment.ToString("C")}\t\t{remainingBalance.ToString("C")}");
                }
            }
            else
            {
                Console.WriteLine("Loan not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Loan ID format.");
        }
    }
}

public class Loan
{
    public Guid Id { get; set; }
    public string BorrowerName { get; set; }
    public decimal Amount { get; set; }
    public double AnnualInterestRate { get; set; }
    public int TermMonths { get; set; }
    public DateTime StartDate { get; set; }
}