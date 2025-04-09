using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class HomeRenovationCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Home Renovation Cost Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Home Renovation Cost Calculator...");

        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string configPath = Path.Combine(dataFolder, "renovation_config.json");

            RenovationProject project;
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                project = JsonSerializer.Deserialize<RenovationProject>(json);
                Console.WriteLine("Loaded existing project configuration.");
            }
            else
            {
                project = new RenovationProject();
                Console.WriteLine("Created new project configuration.");
            }

            Console.WriteLine("Current Project Details:");
            DisplayProjectDetails(project);

            bool continueEditing = true;
            while (continueEditing)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Edit Venue Cost");
                Console.WriteLine("2. Edit Catering Cost");
                Console.WriteLine("3. Edit Decorations Cost");
                Console.WriteLine("4. Calculate Total");
                Console.WriteLine("5. Save and Exit");
                Console.Write("Select an option: ");

                string input = Console.ReadLine();
                if (int.TryParse(input, out int choice))
                {
                    switch (choice)
                    {
                        case 1:
                            project.VenueCost = GetDecimalInput("Enter venue cost: ");
                            break;
                        case 2:
                            project.CateringCost = GetDecimalInput("Enter catering cost: ");
                            break;
                        case 3:
                            project.DecorationsCost = GetDecimalInput("Enter decorations cost: ");
                            break;
                        case 4:
                            Console.WriteLine("\nTotal Project Cost: " + project.CalculateTotal().ToString("C"));
                            break;
                        case 5:
                            continueEditing = false;
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            }

            string updatedJson = JsonSerializer.Serialize(project, options);
            File.WriteAllText(configPath, updatedJson);
            Console.WriteLine("Project saved successfully.");

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private decimal GetDecimalInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (decimal.TryParse(input, out decimal result))
            {
                return result;
            }
            Console.WriteLine("Invalid input. Please enter a valid decimal number.");
        }
    }

    private void DisplayProjectDetails(RenovationProject project)
    {
        Console.WriteLine("Venue Cost: " + project.VenueCost.ToString("C"));
        Console.WriteLine("Catering Cost: " + project.CateringCost.ToString("C"));
        Console.WriteLine("Decorations Cost: " + project.DecorationsCost.ToString("C"));
    }
}

public class RenovationProject
{
    public decimal VenueCost { get; set; }
    public decimal CateringCost { get; set; }
    public decimal DecorationsCost { get; set; }

    public decimal CalculateTotal()
    {
        return VenueCost + CateringCost + DecorationsCost;
    }
}