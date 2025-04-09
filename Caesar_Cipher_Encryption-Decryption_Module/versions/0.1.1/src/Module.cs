using SelfEvolvingSoftware.Interfaces;
using System;
using System.IO;
using System.Text;

public class CaesarCipherModule : IGeneratedModule
{
    public string Name { get; set; } = "Caesar Cipher Encryption/Decryption Module";

    public bool Main(string dataFolder)
    {
        Console.WriteLine("Caesar Cipher Module is running...");
        
        try
        {
            string settingsPath = Path.Combine(dataFolder, "caesar_settings.json");
            int shift = 3; // Default shift value
            
            if (File.Exists(settingsPath))
            {
                string settingsJson = File.ReadAllText(settingsPath);
                shift = int.Parse(settingsJson);
            }
            else
            {
                File.WriteAllText(settingsPath, shift.ToString());
            }
            
            Console.WriteLine("Enter text to encrypt:");
            string plainText = Console.ReadLine();
            
            string encryptedText = Encrypt(plainText, shift);
            Console.WriteLine("Encrypted text: " + encryptedText);
            
            Console.WriteLine("Enter text to decrypt:");
            string cipherText = Console.ReadLine();
            
            string decryptedText = Decrypt(cipherText, shift);
            Console.WriteLine("Decrypted text: " + decryptedText);
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
            return false;
        }
    }
    
    private string Encrypt(string text, int shift)
    {
        StringBuilder result = new StringBuilder();
        
        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                char offset = char.IsUpper(c) ? 'A' : 'a';
                result.Append((char)(((c + shift - offset) % 26) + offset));
            }
            else
            {
                result.Append(c);
            }
        }
        
        return result.ToString();
    }
    
    private string Decrypt(string text, int shift)
    {
        return Encrypt(text, 26 - shift);
    }
}