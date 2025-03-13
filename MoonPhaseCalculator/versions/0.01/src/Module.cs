namespace GenerativeSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;

public class MoonPhaseModule : IGeneratedModule
{
    public string Name { get; set; } = "Moon Phase Calculator";

    public bool Main(string dataFolder)
    {
        try
        {
            // Calculate moon phase
            DateTime currentDate = DateTime.UtcNow;
            double synodicMonth = 29.53058867;
            double diff = currentDate.AddDays(-currentDate.DayOfYear).DaysSinceEpoch();
            double days = currentDate.DayOfYear;
            double phase = Math.Floor(13 * (diff + days) / synodicMonth);

            string moonPhase;
            switch (phase)
            {
                case 0: moonPhase = "First Quarter"; break;
                case 1: moonPhase = "Waxing Gibbous"; break;
                case 2: moonPhase = "Full Moon"; break;
                case 3: moonPhase = "Last Quarter"; break;
                case 4: moonPhase = "Waning Crescent"; break;
                default: moonPhase = "New Moon"; break;
            }

            // Display result
            Console.WriteLine("Current Moon Phase: " + moonPhase);

            // Save data to JSON file
            var result = new {
                Date = currentDate.ToString("yyyy-MM-dd HH:mm:ss UTC"),
                Phase = moonPhase
            };

            string jsonPath = Path.Combine(dataFolder, "moon_phase.json");
            File.WriteAllText(jsonPath, JsonSerializer.Serialize(result));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error calculating moon phase: " + ex.Message);
            return false;
        }
    }
}

public static class DateTimeExtensions
{
    public static double DaysSinceEpoch(this DateTime date)
    {
        var epoch = new DateTime(2000, 1, 1);
        return (date - epoch).TotalDays;
    }
}
