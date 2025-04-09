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
    private Random _random;
    private List<List<bool>> _maze;
    private List<List<bool>> _solution;

    public MazeGeneratorSolver()
    {
        _random = new Random();
    }

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Maze Generator and Solver module is running...");
        Console.WriteLine("Generating a random maze...");

        _width = 10;
        _height = 10;
        GenerateMaze();

        Console.WriteLine("Maze generated successfully.");
        Console.WriteLine("Solving the maze...");

        SolveMaze();

        Console.WriteLine("Maze solved successfully.");

        SaveMazeToFile(dataFolder);
        SaveSolutionToFile(dataFolder);

        Console.WriteLine("Maze and solution saved to files.");

        return true;
    }

    private void GenerateMaze()
    {
        _maze = new List<List<bool>>();
        for (int i = 0; i < _height; i++)
        {
            _maze.Add(new List<bool>());
            for (int j = 0; j < _width; j++)
            {
                _maze[i].Add(_random.Next(0, 2) == 0);
            }
        }

        _maze[0][0] = true;
        _maze[_height - 1][_width - 1] = true;
    }

    private void SolveMaze()
    {
        _solution = new List<List<bool>>();
        for (int i = 0; i < _height; i++)
        {
            _solution.Add(new List<bool>());
            for (int j = 0; j < _width; j++)
            {
                _solution[i].Add(false);
            }
        }

        SolveMazeRecursive(0, 0);
    }

    private bool SolveMazeRecursive(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height || !_maze[y][x] || _solution[y][x])
        {
            return false;
        }

        _solution[y][x] = true;

        if (x == _width - 1 && y == _height - 1)
        {
            return true;
        }

        if (SolveMazeRecursive(x + 1, y) || SolveMazeRecursive(x, y + 1) ||
            SolveMazeRecursive(x - 1, y) || SolveMazeRecursive(x, y - 1))
        {
            return true;
        }

        _solution[y][x] = false;
        return false;
    }

    private void SaveMazeToFile(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, "maze.json");
        string json = JsonSerializer.Serialize(_maze);
        File.WriteAllText(filePath, json);
    }

    private void SaveSolutionToFile(string dataFolder)
    {
        string filePath = Path.Combine(dataFolder, "solution.json");
        string json = JsonSerializer.Serialize(_solution);
        File.WriteAllText(filePath, json);
    }
}