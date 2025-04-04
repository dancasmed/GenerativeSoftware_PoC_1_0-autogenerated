using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class SudokuGeneratorModule
{
    public string Name { get; set; } = "Sudoku Generator Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Sudoku Generator Module is running...");
        Console.WriteLine("Generating a random Sudoku puzzle...");

        try
        {
            int[,] puzzle = GenerateSudokuPuzzle();
            string puzzleJson = JsonSerializer.Serialize(puzzle);
            
            string filePath = Path.Combine(dataFolder, "sudoku_puzzle.json");
            File.WriteAllText(filePath, puzzleJson);
            
            Console.WriteLine("Sudoku puzzle generated and saved successfully.");
            Console.WriteLine("Puzzle saved to: " + filePath);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating Sudoku puzzle: " + ex.Message);
            return false;
        }
    }

    private int[,] GenerateSudokuPuzzle()
    {
        int[,] grid = new int[9, 9];
        Random random = new Random();
        
        // Fill diagonal 3x3 boxes
        for (int box = 0; box < 9; box += 3)
        {
            FillDiagonalBox(grid, box, box, random);
        }
        
        // Solve the complete Sudoku
        SolveSudoku(grid, random);
        
        // Remove some numbers to create a puzzle
        int cellsToRemove = random.Next(40, 60);
        for (int i = 0; i < cellsToRemove; i++)
        {
            int row = random.Next(0, 9);
            int col = random.Next(0, 9);
            
            // Ensure we don't remove from already empty cells
            while (grid[row, col] == 0)
            {
                row = random.Next(0, 9);
                col = random.Next(0, 9);
            }
            
            grid[row, col] = 0;
        }
        
        return grid;
    }

    private void FillDiagonalBox(int[,] grid, int startRow, int startCol, Random random)
    {
        int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        ShuffleArray(numbers, random);
        
        int index = 0;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                grid[startRow + i, startCol + j] = numbers[index++];
            }
        }
    }

    private void ShuffleArray(int[] array, Random random)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    private bool SolveSudoku(int[,] grid, Random random)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (grid[row, col] == 0)
                {
                    int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    ShuffleArray(numbers, random);
                    
                    foreach (int num in numbers)
                    {
                        if (IsValidPlacement(grid, row, col, num))
                        {
                            grid[row, col] = num;
                            
                            if (SolveSudoku(grid, random))
                            {
                                return true;
                            }
                            
                            grid[row, col] = 0;
                        }
                    }
                    
                    return false;
                }
            }
        }
        
        return true;
    }

    private bool IsValidPlacement(int[,] grid, int row, int col, int num)
    {
        // Check row
        for (int i = 0; i < 9; i++)
        {
            if (grid[row, i] == num)
            {
                return false;
            }
        }
        
        // Check column
        for (int i = 0; i < 9; i++)
        {
            if (grid[i, col] == num)
            {
                return false;
            }
        }
        
        // Check 3x3 box
        int boxStartRow = row - row % 3;
        int boxStartCol = col - col % 3;
        
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (grid[boxStartRow + i, boxStartCol + j] == num)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
}