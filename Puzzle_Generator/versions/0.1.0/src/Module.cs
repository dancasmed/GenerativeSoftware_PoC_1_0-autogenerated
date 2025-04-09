using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class PuzzleGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Puzzle Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Puzzle Generator module is running.");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            Console.WriteLine("Generating a Sudoku puzzle...");
            int[,] sudokuPuzzle = GenerateSudoku();
            string sudokuFilePath = Path.Combine(dataFolder, "sudoku_puzzle.json");
            SavePuzzleToFile(sudokuPuzzle, sudokuFilePath);
            Console.WriteLine("Sudoku puzzle saved to " + sudokuFilePath);

            Console.WriteLine("Generating a Crossword puzzle...");
            char[,] crosswordPuzzle = GenerateCrossword();
            string crosswordFilePath = Path.Combine(dataFolder, "crossword_puzzle.json");
            SavePuzzleToFile(crosswordPuzzle, crosswordFilePath);
            Console.WriteLine("Crossword puzzle saved to " + crosswordFilePath);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private int[,] GenerateSudoku()
    {
        int[,] grid = new int[9, 9];
        Random random = new Random();

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                grid[i, j] = random.Next(1, 10);
            }
        }

        return grid;
    }

    private char[,] GenerateCrossword()
    {
        char[,] grid = new char[10, 10];
        Random random = new Random();
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                grid[i, j] = letters[random.Next(letters.Length)];
            }
        }

        return grid;
    }

    private void SavePuzzleToFile<T>(T puzzle, string filePath)
    {
        string json = JsonSerializer.Serialize(puzzle);
        File.WriteAllText(filePath, json);
    }
}