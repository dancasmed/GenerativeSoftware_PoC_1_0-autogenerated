using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MoonPhaseModule : IGeneratedModule
{
    public string Name { get; set; } = "Moon Phase Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Moon Phase Module is running...");
        
        try
        {
            DateTime today = DateTime.Today;
            double moonPhase = CalculateMoonPhase(today);
            string phaseName = GetMoonPhaseName(moonPhase);
            
            Console.WriteLine("Today's moon phase: " + phaseName);
            
            // Save to JSON file
            string filePath = Path.Combine(dataFolder, "moon_phase.json");
            var moonData = new { Date = today, Phase = moonPhase, PhaseName = phaseName };
            string jsonData = JsonSerializer.Serialize(moonData);
            File.WriteAllText(filePath, jsonData);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error calculating moon phase: " + ex.Message);
            return false;
        }
    }
    
    private double CalculateMoonPhase(DateTime date)
    {
        // Simplified moon phase calculation
        // Based on the approximate 29.53-day lunar cycle
        DateTime knownNewMoon = new DateTime(2000, 1, 6); // Arbitrary known new moon date
        double daysSinceKnownNewMoon = (date - knownNewMoon).TotalDays;
        double lunarCycle = 29.530588853; // Average length of a lunar cycle in days
        
        // Calculate the current phase (0 to 1)
        double phase = (daysSinceKnownNewMoon % lunarCycle) / lunarCycle;
        
        return phase;
    }
    
    private string GetMoonPhaseName(double phase)
    {
        if (phase < 0.03 || phase >= 0.97) return "New Moon";
        if (phase < 0.22) return "Waxing Crescent";
        if (phase < 0.28) return "First Quarter";
        if (phase < 0.47) return "Waxing Gibbous";
        if (phase < 0.53) return "Full Moon";
        if (phase < 0.72) return "Waning Gibbous";
        if (phase < 0.78) return "Last Quarter";
        return "Waning Crescent";
    }
}