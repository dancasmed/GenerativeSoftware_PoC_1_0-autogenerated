using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

public class DownloadTimeCalculator : IGeneratedModule
{
    public string Name { get; set; } = "Download Time Calculator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Download Time Calculator module is running.");

        string configFilePath = Path.Combine(dataFolder, "download_config.json");
        string resultFilePath = Path.Combine(dataFolder, "download_result.json");

        try
        {
            if (!File.Exists(configFilePath))
            {
                Console.WriteLine("Configuration file not found. Creating a default one.");
                CreateDefaultConfigFile(configFilePath);
                Console.WriteLine("Please edit the configuration file and run the module again.");
                return false;
            }

            var config = ReadConfigFile(configFilePath);
            if (config == null)
            {
                Console.WriteLine("Failed to read configuration file.");
                return false;
            }

            if (string.IsNullOrEmpty(config.FileUrl))
            {
                Console.WriteLine("File URL is not specified in the configuration.");
                return false;
            }

            Console.WriteLine("Calculating download time...");
            var result = CalculateDownloadTime(config.FileUrl, config.DownloadSpeedMbps);
            if (result == null)
            {
                Console.WriteLine("Failed to calculate download time.");
                return false;
            }

            SaveResult(resultFilePath, result);
            Console.WriteLine("Download time calculation completed successfully.");
            Console.WriteLine("File size: " + FormatFileSize(result.FileSizeBytes));
            Console.WriteLine("Estimated download time: " + FormatTimeSpan(result.EstimatedTime));

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return false;
        }
    }

    private DownloadConfig ReadConfigFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<DownloadConfig>(json);
        }
        catch
        {
            return null;
        }
    }

    private void CreateDefaultConfigFile(string filePath)
    {
        var defaultConfig = new DownloadConfig
        {
            FileUrl = "",
            DownloadSpeedMbps = 10.0
        };

        string json = JsonSerializer.Serialize(defaultConfig, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private DownloadResult CalculateDownloadTime(string fileUrl, double downloadSpeedMbps)
    {
        try
        {
            var request = WebRequest.Create(fileUrl);
            request.Method = "HEAD";

            using (var response = request.GetResponse())
            {
                long fileSize = response.ContentLength;
                if (fileSize <= 0)
                {
                    return null;
                }

                double speedBytesPerSecond = (downloadSpeedMbps * 125000); // Convert Mbps to bytes/sec
                double seconds = fileSize / speedBytesPerSecond;
                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);

                return new DownloadResult
                {
                    FileSizeBytes = fileSize,
                    DownloadSpeedMbps = downloadSpeedMbps,
                    EstimatedTime = timeSpan,
                    FileUrl = fileUrl,
                    CalculationDate = DateTime.Now
                };
            }
        }
        catch
        {
            return null;
        }
    }

    private void SaveResult(string filePath, DownloadResult result)
    {
        string json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return string.Format("{0:0.##} {1}", size, sizes[order]);
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        if (timeSpan.TotalHours >= 1)
        {
            return string.Format("{0}h {1}m {2}s", (int)timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return string.Format("{0}m {1}s", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
        }
        else
        {
            return string.Format("{0}s", timeSpan.Seconds);
        }
    }
}

public class DownloadConfig
{
    public string FileUrl { get; set; }
    public double DownloadSpeedMbps { get; set; }
}

public class DownloadResult
{
    public long FileSizeBytes { get; set; }
    public double DownloadSpeedMbps { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public string FileUrl { get; set; }
    public DateTime CalculationDate { get; set; }
}