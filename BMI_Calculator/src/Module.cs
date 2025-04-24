using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class BMICalculatorModule : IGeneratedModule
{
    public string Name { get; set; } = "BMI Calculator Module";

    public bool Main(string dataFolder)
    {
        Directory.CreateDirectory(dataFolder);

        UserSession session = new UserSession
        {
            UserId = Guid.NewGuid().ToString(),
            Timestamp = DateTime.Now
        };

        bool continueLoop = true;
        while (continueLoop)
        {
            string units = "";
            while (true)
            {
                Console.WriteLine("Choose units (metric/imperial):");
                units = Console.ReadLine().Trim().ToLower();
                if (units == "metric" || units == "imperial")
                    break;
                Console.WriteLine("Invalid unit. Please enter 'metric' or 'imperial'.");
            }

            float height = 0;
            float weight = 0;

            if (units == "metric")
            {
                while (true)
                {
                    Console.WriteLine("Enter height in centimeters:");
                    string heightInput = Console.ReadLine();
                    if (float.TryParse(heightInput, out height) && height > 50 && height <= 300)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid height. Please enter a positive number between 50 and 300 cm.");
                }

                while (true)
                {
                    Console.WriteLine("Enter weight in kilograms:");
                    string weightInput = Console.ReadLine();
                    if (float.TryParse(weightInput, out weight) && weight > 20 && weight <= 300)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid weight. Please enter a positive number between 20 and 300 kg.");
                }
            }
            else
            {
                int feet = 0;
                int inches = 0;

                while (true)
                {
                    Console.WriteLine("Enter feet:");
                    string feetInput = Console.ReadLine();
                    if (int.TryParse(feetInput, out feet) && feet >= 4 && feet <= 8)
                        break;
                    Console.WriteLine("Invalid feet. Please enter a number between 4 and 8.");
                }

                while (true)
                {
                    Console.WriteLine("Enter inches:");
                    string inchesInput = Console.ReadLine();
                    if (int.TryParse(inchesInput, out inches) && inches >= 0 && inches < 12)
                        break;
                    Console.WriteLine("Invalid inches. Please enter a number between 0 and 11.");
                }

                height = (feet * 12) + inches;

                while (true)
                {
                    Console.WriteLine("Enter weight in pounds:");
                    string weightInput = Console.ReadLine();
                    if (float.TryParse(weightInput, out weight) && weight > 50 && weight <= 660)
                    {
                        break;
                    }
                    Console.WriteLine("Invalid weight. Please enter a positive number between 50 and 660 lbs.");
                }
            }

            float bmi = 0;
            if (units == "metric")
            {
                float heightMeters = height / 100f;
                bmi = weight / (heightMeters * heightMeters);
            }
            else
            {
                bmi = (weight * 703) / (height * height);
            }

            string category;
            string recommendation;

            if (bmi < 18.5f)
            {
                category = "Underweight";
                recommendation = "Recommendation: Consult a healthcare provider for gaining weight.";
            }
            else if (bmi < 25)
            {
                category = "Normal weight";
                recommendation = "Recommendation: Maintain a healthy diet and exercise routine.";
            }
            else if (bmi < 30)
            {
                category = "Overweight";
                recommendation = "Recommendation: Consider dietary changes and increased physical activity.";
            }
            else
            {
                category = "Obese";
                recommendation = "Recommendation: Consult a healthcare provider for a weight management plan.";
            }

            Console.WriteLine("BMI: " + bmi.ToString("F2"));
            Console.WriteLine("Category: " + category);
            Console.WriteLine(recommendation);

            UserInput userInput = new UserInput
            {
                Height = height,
                Weight = weight,
                Units = units
            };

            BMIResult result = new BMIResult
            {
                BMI = bmi,
                Category = category,
                Recommendation = recommendation
            };

            session.Inputs.Add(userInput);
            session.Results.Add(result);

            Console.WriteLine("Calculate another BMI? (yes/no)");
            string response = Console.ReadLine().Trim().ToLower();
            if (response != "yes")
                continueLoop = false;
        }

        string sessionJson = JsonSerializer.Serialize(session);
        string filePath = Path.Combine(dataFolder, "session_" + session.UserId + ".json");
        File.WriteAllText(filePath, sessionJson);

        return true;
    }
}

public class UserInput
{
    public float Height { get; set; }
    public float Weight { get; set; }
    public string Units { get; set; }
}

public class BMIResult
{
    public float BMI { get; set; }
    public string Category { get; set; }
    public string Recommendation { get; set; }
}

public class UserSession
{
    public string UserId { get; set; }
    public List<UserInput> Inputs { get; set; } = new List<UserInput>();
    public List<BMIResult> Results { get; set; } = new List<BMIResult>();
    public DateTime Timestamp { get; set; }
}
