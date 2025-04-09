using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class MazeGeneratorSolver : IGeneratedModule
{
    public string Name { get; set; } = "Maze Generator and Solver";

    private int _width;
    private int _height;
    private int[,] _maze;
    private Random _random;

    public MazeGeneratorSolver()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Maze Generator and Solver module...");
        
        _width = 10;
        _height = 10;
        _maze = new int[_width, _height];

        GenerateMaze();
        Console.WriteLine("Maze generated successfully.");

        string mazeFilePath = Path.Combine(dataFolder, "maze.json");
        SaveMazeToFile(mazeFilePath);
        Console.WriteLine("Maze saved to " + mazeFilePath);

        bool isSolved = SolveMaze(0, 0, _width - 1, _height - 1);
        if (isSolved)
        {
            Console.WriteLine("Maze solved successfully.");
            string solutionFilePath = Path.Combine(dataFolder, "maze_solution.json");
            SaveMazeToFile(solutionFilePath);
            Console.WriteLine("Maze solution saved to " + solutionFilePath);
        }
        else
        {
            Console.WriteLine("Failed to solve the maze.");
        }

        return isSolved;
    }

    private void GenerateMaze()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _maze[x, y] = _random.Next(0, 2); // 0 for wall, 1 for path
            }
        }
        // Ensure start and end are paths
        _maze[0, 0] = 1;
        _maze[_width - 1, _height - 1] = 1;
    }

    private bool SolveMaze(int startX, int startY, int endX, int endY)
    {
        var visited = new bool[_width, _height];
        return SolveMazeUtil(startX, startY, endX, endY, visited);
    }

    private bool SolveMazeUtil(int x, int y, int endX, int endY, bool[,] visited)
    {
        if (x == endX && y == endY)
        {
            return true;
        }

        if (IsValidMove(x, y, visited))
        {
            visited[x, y] = true;

            // Mark the path
            _maze[x, y] = 2;

            // Move right
            if (SolveMazeUtil(x + 1, y, endX, endY, visited))
                return true;

            // Move down
            if (SolveMazeUtil(x, y + 1, endX, endY, visited))
                return true;

            // Move left
            if (SolveMazeUtil(x - 1, y, endX, endY, visited))
                return true;

            // Move up
            if (SolveMazeUtil(x, y - 1, endX, endY, visited))
                return true;

            // Backtrack
            _maze[x, y] = 1;
        }

        return false;
    }

    private bool IsValidMove(int x, int y, bool[,] visited)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height && _maze[x, y] == 1 && !visited[x, y];
    }

    private void SaveMazeToFile(string filePath)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(_maze, options);
        File.WriteAllText(filePath, jsonString);
    }
}