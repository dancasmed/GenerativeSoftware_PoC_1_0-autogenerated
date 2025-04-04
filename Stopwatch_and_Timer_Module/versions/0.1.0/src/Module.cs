using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;

public class StopwatchTimerModule : IGeneratedModule
{
    public string Name { get; set; } = "Stopwatch and Timer Module";
    
    private bool _isRunning;
    private DateTime _startTime;
    private TimeSpan _elapsedTime;
    private Timer _timer;
    private TimeSpan _timerDuration;
    private bool _isTimerRunning;
    
    public StopwatchTimerModule()
    {
    }
    
    public bool Main(string dataFolder)
    {
        Console.WriteLine("Stopwatch and Timer Module is running.");
        Console.WriteLine("Commands: start, stop, reset, lap, settimer [seconds], starttimer, stoptimer, exit");
        
        _isRunning = false;
        _isTimerRunning = false;
        _elapsedTime = TimeSpan.Zero;
        
        string input;
        do
        {
            Console.Write("> ");
            input = Console.ReadLine()?.ToLower().Trim() ?? string.Empty;
            
            switch (input)
            {
                case "start":
                    StartStopwatch();
                    break;
                case "stop":
                    StopStopwatch();
                    break;
                case "reset":
                    ResetStopwatch();
                    break;
                case "lap":
                    LapStopwatch();
                    break;
                case var _ when input.StartsWith("settimer"):
                    SetTimer(input);
                    break;
                case "starttimer":
                    StartTimer();
                    break;
                case "stoptimer":
                    StopTimer();
                    break;
                case "exit":
                    break;
                default:
                    Console.WriteLine("Invalid command.");
                    break;
            }
            
        } while (input != "exit");
        
        return true;
    }
    
    private void StartStopwatch()
    {
        if (_isRunning)
        {
            Console.WriteLine("Stopwatch is already running.");
            return;
        }
        
        _isRunning = true;
        _startTime = DateTime.Now - _elapsedTime;
        Console.WriteLine("Stopwatch started.");
    }
    
    private void StopStopwatch()
    {
        if (!_isRunning)
        {
            Console.WriteLine("Stopwatch is not running.");
            return;
        }
        
        _elapsedTime = DateTime.Now - _startTime;
        _isRunning = false;
        Console.WriteLine("Stopwatch stopped. Elapsed time: " + _elapsedTime.ToString("g"));
    }
    
    private void ResetStopwatch()
    {
        _isRunning = false;
        _elapsedTime = TimeSpan.Zero;
        Console.WriteLine("Stopwatch reset.");
    }
    
    private void LapStopwatch()
    {
        if (!_isRunning)
        {
            Console.WriteLine("Stopwatch is not running.");
            return;
        }
        
        TimeSpan lapTime = DateTime.Now - _startTime;
        Console.WriteLine("Lap time: " + lapTime.ToString("g"));
    }
    
    private void SetTimer(string input)
    {
        string[] parts = input.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int seconds) || seconds <= 0)
        {
            Console.WriteLine("Invalid timer duration. Usage: settimer [seconds]");
            return;
        }
        
        _timerDuration = TimeSpan.FromSeconds(seconds);
        Console.WriteLine("Timer set for " + _timerDuration.ToString("g"));
    }
    
    private void StartTimer()
    {
        if (_timerDuration == TimeSpan.Zero)
        {
            Console.WriteLine("Timer duration not set. Use 'settimer [seconds]' first.");
            return;
        }
        
        if (_isTimerRunning)
        {
            Console.WriteLine("Timer is already running.");
            return;
        }
        
        _isTimerRunning = true;
        DateTime endTime = DateTime.Now + _timerDuration;
        _timer = new Timer(_ =>
        {
            TimeSpan remaining = endTime - DateTime.Now;
            if (remaining <= TimeSpan.Zero)
            {
                _isTimerRunning = false;
                Console.WriteLine("Timer completed!");
                _timer?.Dispose();
            }
            else
            {
                Console.WriteLine("Time remaining: " + remaining.ToString("g"));
            }
        }, null, 0, 1000);
        
        Console.WriteLine("Timer started.");
    }
    
    private void StopTimer()
    {
        if (!_isTimerRunning)
        {
            Console.WriteLine("Timer is not running.");
            return;
        }
        
        _isTimerRunning = false;
        _timer?.Dispose();
        Console.WriteLine("Timer stopped.");
    }
}