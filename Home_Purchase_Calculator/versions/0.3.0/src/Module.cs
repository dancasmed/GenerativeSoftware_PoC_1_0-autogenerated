using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomePurchaseCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Purchase Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Home Purchase Calculator module is running.");
        
        try
        {
            string configPath = Path.Combine(dataFolder, "home_purchase_config.json");
            string resultPath = Path.Combine(dataFolder, "home_purchase_result.json");

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
                Console.WriteLine("Default configuration file created. Please fill in the values and run again.");
                return false;
            }

            var config = LoadConfig(configPath);
            var result = CalculateTotalCost(config);
            
            SaveResult(resultPath, result);
            
            Console.WriteLine("Calculation completed successfully.");
            Console.WriteLine("Results saved to: " + resultPath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }

    private HomePurchaseConfig LoadConfig(string path)
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<HomePurchaseConfig>(json);
    }

    private void SaveResult(string path, HomePurchaseResult result)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(result, options);
        File.WriteAllText(path, json);
    }

    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new HomePurchaseConfig
        {
            HomePrice = 300000,
            DownPaymentPercentage = 20,
            InterestRate = 3.5,
            LoanTermYears = 30,
            PropertyTaxRate = 1.2,
            HomeownersInsurance = 1000,
            ClosingCosts = 5000
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(defaultConfig, options);
        File.WriteAllText(path, json);
    }

    private HomePurchaseResult CalculateTotalCost(HomePurchaseConfig config)
    {
        double downPayment = config.HomePrice * (config.DownPaymentPercentage / 100);
        double loanAmount = config.HomePrice - downPayment;
        
        double monthlyInterestRate = config.InterestRate / 100 / 12;
        int numberOfPayments = config.LoanTermYears * 12;
        
        double monthlyPayment = loanAmount * 
            (monthlyInterestRate * Math.Pow(1 + monthlyInterestRate, numberOfPayments)) / 
            (Math.Pow(1 + monthlyInterestRate, numberOfPayments) - 1);
        
        double annualPropertyTax = config.HomePrice * (config.PropertyTaxRate / 100);
        double monthlyPropertyTax = annualPropertyTax / 12;
        double monthlyInsurance = config.HomeownersInsurance / 12;
        
        double totalMonthlyPayment = monthlyPayment + monthlyPropertyTax + monthlyInsurance;
        double totalLoanCost = monthlyPayment * numberOfPayments;
        double totalInterest = totalLoanCost - loanAmount;
        
        double totalCost = config.HomePrice + totalInterest + config.ClosingCosts + 
                          (annualPropertyTax * config.LoanTermYears) + 
                          (config.HomeownersInsurance * config.LoanTermYears);
        
        return new HomePurchaseResult
        {
            DownPayment = downPayment,
            LoanAmount = loanAmount,
            MonthlyPayment = monthlyPayment,
            MonthlyPropertyTax = monthlyPropertyTax,
            MonthlyInsurance = monthlyInsurance,
            TotalMonthlyPayment = totalMonthlyPayment,
            TotalInterest = totalInterest,
            TotalLoanCost = totalLoanCost,
            TotalCost = totalCost,
            CalculationDate = DateTime.Now
        };
    }
}

public class HomePurchaseConfig
{
    public double HomePrice { get; set; }
    public double DownPaymentPercentage { get; set; }
    public double InterestRate { get; set; }
    public int LoanTermYears { get; set; }
    public double PropertyTaxRate { get; set; }
    public double HomeownersInsurance { get; set; }
    public double ClosingCosts { get; set; }
}

public class HomePurchaseResult
{
    public double DownPayment { get; set; }
    public double LoanAmount { get; set; }
    public double MonthlyPayment { get; set; }
    public double MonthlyPropertyTax { get; set; }
    public double MonthlyInsurance { get; set; }
    public double TotalMonthlyPayment { get; set; }
    public double TotalInterest { get; set; }
    public double TotalLoanCost { get; set; }
    public double TotalCost { get; set; }
    public DateTime CalculationDate { get; set; }
}