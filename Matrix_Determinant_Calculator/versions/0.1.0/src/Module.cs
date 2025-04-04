using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MatrixDeterminantCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Matrix Determinant Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Matrix Determinant Calculator module is running.");
        
        try
        {
            // Define a sample 2x2 matrix
            double[,] matrix = new double[2, 2] { { 1, 2 }, { 3, 4 } };
            
            // Calculate the determinant
            double determinant = (matrix[0, 0] * matrix[1, 1]) - (matrix[0, 1] * matrix[1, 0]);
            
            // Display the result
            Console.WriteLine("The determinant of the matrix is: " + determinant);
            
            // Save the result to a JSON file in the data folder
            var result = new
            {
                Matrix = new double[2][] { new double[] { matrix[0, 0], matrix[0, 1] }, new double[] { matrix[1, 0], matrix[1, 1] } },
                Determinant = determinant
            };
            
            string jsonResult = JsonSerializer.Serialize(result);
            string filePath = Path.Combine(dataFolder, "determinant_result.json");
            File.WriteAllText(filePath, jsonResult);
            
            Console.WriteLine("Result saved to " + filePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }
}