using SelfEvolvingSoftware.Interfaces;
using System;
using System.Threading;

public class TrafficLightSimulator : IGeneratedModule
{
    public string Name { get; set; } = "Traffic Light Simulator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Starting Traffic Light Simulator...");
        Console.WriteLine("Press Ctrl+C to exit.");

        try
        {
            while (true)
            {
                ChangeLight("Red", ConsoleColor.Red);
                Thread.Sleep(3000);

                ChangeLight("Yellow", ConsoleColor.Yellow);
                Thread.Sleep(1000);

                ChangeLight("Green", ConsoleColor.Green);
                Thread.Sleep(3000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private void ChangeLight(string color, ConsoleColor consoleColor)
    {
        Console.ForegroundColor = consoleColor;
        Console.WriteLine("Traffic light is now: " + color);
        Console.ResetColor();
    }
}