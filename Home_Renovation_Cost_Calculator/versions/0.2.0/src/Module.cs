using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class HomeRenovationCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Cost Calculator";

    private class ProjectData
    {
        public List<Contractor> Contractors { get; set; } = new List<Contractor>();
        public List<Material> Materials { get; set; } = new List<Material>();
    }

    private class Contractor
    {
        public string Name { get; set; }
        public decimal HourlyRate { get; set; }
        public int HoursWorked { get; set; }
    }

    private class Material
    {
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Cost Calculator...");
        
        string dataFilePath = Path.Combine(dataFolder, "project_data.json");
        ProjectData projectData;

        try
        {
            if (File.Exists(dataFilePath))
            {
                string jsonData = File.ReadAllText(dataFilePath);
                projectData = JsonSerializer.Deserialize<ProjectData>(jsonData);
                Console.WriteLine("Loaded existing project data.");
            }
            else
            {
                projectData = new ProjectData();
                Console.WriteLine("Created new project data file.");
            }

            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nMain Menu:");
                Console.WriteLine("1. Add Contractor");
                Console.WriteLine("2. Add Material");
                Console.WriteLine("3. View Current Costs");
                Console.WriteLine("4. Save and Exit");
                Console.Write("Select an option: ");
                
                string input = Console.ReadLine();
                
                switch (input)
                {
                    case "1":
                        AddContractor(projectData);
                        break;
                    case "2":
                        AddMaterial(projectData);
                        break;
                    case "3":
                        DisplayCurrentCosts(projectData);
                        break;
                    case "4":
                        continueRunning = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }

            string updatedJsonData = JsonSerializer.Serialize(projectData);
            File.WriteAllText(dataFilePath, updatedJsonData);
            Console.WriteLine("Project data saved successfully.");
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void AddContractor(ProjectData projectData)
    {
        Console.Write("Enter contractor name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter hourly rate: ");
        decimal hourlyRate = decimal.Parse(Console.ReadLine());
        
        Console.Write("Enter hours worked: ");
        int hoursWorked = int.Parse(Console.ReadLine());
        
        projectData.Contractors.Add(new Contractor
        {
            Name = name,
            HourlyRate = hourlyRate,
            HoursWorked = hoursWorked
        });
        
        Console.WriteLine("Contractor added successfully.");
    }

    private void AddMaterial(ProjectData projectData)
    {
        Console.Write("Enter material name: ");
        string name = Console.ReadLine();
        
        Console.Write("Enter unit price: ");
        decimal unitPrice = decimal.Parse(Console.ReadLine());
        
        Console.Write("Enter quantity: ");
        int quantity = int.Parse(Console.ReadLine());
        
        projectData.Materials.Add(new Material
        {
            Name = name,
            UnitPrice = unitPrice,
            Quantity = quantity
        });
        
        Console.WriteLine("Material added successfully.");
    }

    private void DisplayCurrentCosts(ProjectData projectData)
    {
        decimal totalContractorCost = 0;
        decimal totalMaterialCost = 0;
        
        Console.WriteLine("\n--- Contractors ---");
        foreach (var contractor in projectData.Contractors)
        {
            decimal cost = contractor.HourlyRate * contractor.HoursWorked;
            totalContractorCost += cost;
            Console.WriteLine(string.Format("{0}: {1} hours at {2}/hr = {3:C}", 
                contractor.Name, contractor.HoursWorked, contractor.HourlyRate, cost));
        }
        
        Console.WriteLine("\n--- Materials ---");
        foreach (var material in projectData.Materials)
        {
            decimal cost = material.UnitPrice * material.Quantity;
            totalMaterialCost += cost;
            Console.WriteLine(string.Format("{0}: {1} units at {2:C} each = {3:C}", 
                material.Name, material.Quantity, material.UnitPrice, cost));
        }
        
        Console.WriteLine("\n--- Summary ---");
        Console.WriteLine(string.Format("Total Contractor Costs: {0:C}", totalContractorCost));
        Console.WriteLine(string.Format("Total Material Costs: {0:C}", totalMaterialCost));
        Console.WriteLine(string.Format("Total Project Cost: {0:C}", totalContractorCost + totalMaterialCost));
    }
}