using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class SecurePasswordGenerator : IGeneratedModule
{
    public string Name { get; set; } = "Secure Password Generator";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Secure Password Generator module is running...");
        Console.WriteLine("Generating a random secure password...");

        string password = GenerateSecurePassword(16);
        Console.WriteLine("Generated Password: " + password);

        string outputPath = Path.Combine(dataFolder, "generated_password.json");
        File.WriteAllText(outputPath, $"{{\"password\": \"{password}\"}}");
        Console.WriteLine("Password saved to: " + outputPath);

        return true;
    }

    private string GenerateSecurePassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=[]{}|;:,.<>?~\\";
        StringBuilder password = new StringBuilder();

        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];

            while (password.Length < length)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                password.Append(validChars[(int)(num % (uint)validChars.Length)]);
            }
        }

        return password.ToString();
    }
}