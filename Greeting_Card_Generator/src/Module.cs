using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Drawing.Imaging;

public class GreetingCardGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Greeting Card Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Greeting Card Generator module is running.");
        
        try
        {
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
            }

            string settingsPath = Path.Combine(dataFolder, "greeting_settings.json");
            GreetingCardSettings settings;

            if (File.Exists(settingsPath))
            {
                string json = File.ReadAllText(settingsPath);
                settings = JsonSerializer.Deserialize<GreetingCardSettings>(json);
            }
            else
            {
                settings = new GreetingCardSettings
                {
                    Message = "Happy Birthday!",
                    FontName = "Arial",
                    FontSize = 24,
                    TextColor = Color.Black.Name,
                    BackgroundColor = Color.White.Name,
                    ImagePath = ""
                };
                string json = JsonSerializer.Serialize(settings);
                File.WriteAllText(settingsPath, json);
            }

            Console.WriteLine("Generating greeting card with the following settings:");
            Console.WriteLine("Message: " + settings.Message);
            Console.WriteLine("Font: " + settings.FontName + ", Size: " + settings.FontSize);
            Console.WriteLine("Text Color: " + settings.TextColor);
            Console.WriteLine("Background Color: " + settings.BackgroundColor);
            Console.WriteLine("Image Path: " + settings.ImagePath);

            GenerateGreetingCard(settings, dataFolder);
            Console.WriteLine("Greeting card generated successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating greeting card: " + ex.Message);
            return false;
        }
    }

    private void GenerateGreetingCard(GreetingCardSettings settings, string dataFolder)
    {
        int width = 800;
        int height = 600;
        using (Bitmap bitmap = new Bitmap(width, height))
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                Color backgroundColor = Color.FromName(settings.BackgroundColor);
                graphics.Clear(backgroundColor);

                if (!string.IsNullOrEmpty(settings.ImagePath) && File.Exists(settings.ImagePath))
                {
                    using (Image image = Image.FromFile(settings.ImagePath))
                    {
                        graphics.DrawImage(image, new Rectangle(50, 50, width - 100, height - 200));
                    }
                }

                Font font = new Font(settings.FontName, settings.FontSize);
                Color textColor = Color.FromName(settings.TextColor);
                using (SolidBrush brush = new SolidBrush(textColor))
                {
                    SizeF textSize = graphics.MeasureString(settings.Message, font);
                    float x = (width - textSize.Width) / 2;
                    float y = height - 100;
                    graphics.DrawString(settings.Message, font, brush, x, y);
                }
            }

            string outputPath = Path.Combine(dataFolder, "greeting_card.png");
            bitmap.Save(outputPath, ImageFormat.Png);
        }
    }
}

public class GreetingCardSettings
{
    public string Message { get; set; }
    public string FontName { get; set; }
    public int FontSize { get; set; }
    public string TextColor { get; set; }
    public string BackgroundColor { get; set; }
    public string ImagePath { get; set; }
}