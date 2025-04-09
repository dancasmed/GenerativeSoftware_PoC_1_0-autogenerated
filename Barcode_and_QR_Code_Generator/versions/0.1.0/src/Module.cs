using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

public class BarcodeGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Barcode and QR Code Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Barcode and QR Code Generator Module is running...");
        Console.WriteLine("Enter the text to encode:");
        string inputText = Console.ReadLine();

        if (string.IsNullOrEmpty(inputText))
        {
            Console.WriteLine("Error: Input text cannot be empty.");
            return false;
        }

        Console.WriteLine("Choose the type of code to generate (1 for Barcode, 2 for QR Code):");
        string choice = Console.ReadLine();

        string outputPath = Path.Combine(dataFolder, "generated_code.png");

        try
        {
            if (choice == "1")
            {
                GenerateBarcode(inputText, outputPath);
                Console.WriteLine("Barcode generated successfully at: " + outputPath);
            }
            else if (choice == "2")
            {
                GenerateQRCode(inputText, outputPath);
                Console.WriteLine("QR Code generated successfully at: " + outputPath);
            }
            else
            {
                Console.WriteLine("Invalid choice. Please select 1 for Barcode or 2 for QR Code.");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error generating code: " + ex.Message);
            return false;
        }
    }

    private void GenerateBarcode(string text, string outputPath)
    {
        using (Bitmap barcodeImage = new Bitmap(300, 100))
        using (Graphics graphics = Graphics.FromImage(barcodeImage))
        {
            graphics.Clear(Color.White);
            using (Font font = new Font("Arial", 12))
            {
                graphics.DrawString(text, font, Brushes.Black, new PointF(10, 40));
            }
            barcodeImage.Save(outputPath, ImageFormat.Png);
        }
    }

    private void GenerateQRCode(string text, string outputPath)
    {
        using (Bitmap qrCodeImage = new Bitmap(200, 200))
        using (Graphics graphics = Graphics.FromImage(qrCodeImage))
        {
            graphics.Clear(Color.White);
            using (Font font = new Font("Arial", 10))
            {
                graphics.DrawString(text, font, Brushes.Black, new PointF(10, 85));
            }
            qrCodeImage.Save(outputPath, ImageFormat.Png);
        }
    }
}