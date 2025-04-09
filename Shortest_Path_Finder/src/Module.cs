using SelfEvolvingSoftware.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ShortestPathModule : IGeneratedModule
{
    public string Name { get; set; } = "Shortest Path Finder";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Shortest Path Finder module is running.");
        
        try
        {
            string gridFilePath = Path.Combine(dataFolder, "grid.json");
            string pointsFilePath = Path.Combine(dataFolder, "points.json");
            string outputFilePath = Path.Combine(dataFolder, "path_result.json");

            if (!File.Exists(gridFilePath) || !File.Exists(pointsFilePath))
            {
                Console.WriteLine("Required input files (grid.json and points.json) not found in the data folder.");
                return false;
            }

            int[,] grid = LoadGrid(gridFilePath);
            var points = LoadPoints(pointsFilePath);
            
            if (grid == null || points == null)
            {
                Console.WriteLine("Failed to load grid or points data.");
                return false;
            }

            var path = FindShortestPath(grid, points.Start, points.End);
            
            if (path == null)
            {
                Console.WriteLine("No path found between the given points.");
                return false;
            }

            SavePath(outputFilePath, path);
            Console.WriteLine("Shortest path found and saved to path_result.json.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private int[,] LoadGrid(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return System.Text.Json.JsonSerializer.Deserialize<int[,]>(json);
        }
        catch
        {
            return null;
        }
    }

    private Points LoadPoints(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return System.Text.Json.JsonSerializer.Deserialize<Points>(json);
        }
        catch
        {
            return null;
        }
    }

    private void SavePath(string filePath, List<Point> path)
    {
        string json = System.Text.Json.JsonSerializer.Serialize(path);
        File.WriteAllText(filePath, json);
    }

    private List<Point> FindShortestPath(int[,] grid, Point start, Point end)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        if (start.X < 0 || start.X >= rows || start.Y < 0 || start.Y >= cols ||
            end.X < 0 || end.X >= rows || end.Y < 0 || end.Y >= cols)
        {
            return null;
        }

        if (grid[start.X, start.Y] == 0 || grid[end.X, end.Y] == 0)
        {
            return null;
        }

        var queue = new Queue<Point>();
        var visited = new bool[rows, cols];
        var parent = new Point[rows, cols];
        var directions = new[] { new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1) };

        queue.Enqueue(start);
        visited[start.X, start.Y] = true;
        parent[start.X, start.Y] = new Point(-1, -1);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.X == end.X && current.Y == end.Y)
            {
                return ReconstructPath(parent, end);
            }

            foreach (var dir in directions)
            {
                int newX = current.X + dir.X;
                int newY = current.Y + dir.Y;

                if (newX >= 0 && newX < rows && newY >= 0 && newY < cols && 
                    grid[newX, newY] != 0 && !visited[newX, newY])
                {
                    visited[newX, newY] = true;
                    parent[newX, newY] = current;
                    queue.Enqueue(new Point(newX, newY));
                }
            }
        }

        return null;
    }

    private List<Point> ReconstructPath(Point[,] parent, Point end)
    {
        var path = new List<Point>();
        Point current = end;

        while (current.X != -1 && current.Y != -1)
        {
            path.Add(current);
            current = parent[current.X, current.Y];
        }

        path.Reverse();
        return path;
    }
}

public class Points
{
    public Point Start { get; set; }
    public Point End { get; set; }
}

public struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}