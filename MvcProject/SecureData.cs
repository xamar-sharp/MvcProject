using System.Collections;
using System.IO;
using System.Text;
using System;
namespace MvcProject
{
    public class StrictStringException : Exception
    {
        public StrictStringException():base("Invalid password length!")
        {

        }
    }
    public static class SecureData
    {
        public static readonly string ConnectionString;
        public static readonly string AdminPassword;
        static SecureData()
        {
            ConnectionString = $"TrustServerCertificate=True;Server={Environment.MachineName}\\SECURESERVER;Database=MvcProjectBase;User ID=sa;Password=NullForMeCable";
            AdminPassword = System.IO.File.ReadAllText("C:\\adminpassword.txt");
        }
        public static string HashPassword(string password)
        {
            if (password?.Length != 5)
            {
                throw new StrictStringException();
            }
            byte[] byteString = Encoding.UTF8.GetBytes(password);
            BitArray decrypted = new BitArray(byteString);
            byte[] key = new byte[byteString.Length];
            key = File.ReadAllBytes("C:\\securitykeymvc.txt");
            BitArray keyArray = new BitArray(key);
            byte[] encrypted = new byte[byteString.Length];
            decrypted.Xor(keyArray).CopyTo(encrypted, 0);
            return Encoding.UTF8.GetString(encrypted);
        }
        public static string DecryptPassword(string passwordHash)
        {
            byte[] byteString = Encoding.UTF8.GetBytes(passwordHash);
            BitArray encrypted = new BitArray(byteString);
            byte[] key = new byte[byteString.Length];
            key = File.ReadAllBytes("C:\\securitykeymvc.txt");
            BitArray keyArray = new BitArray(key);
            byte[] decrypted = new byte[byteString.Length];
            encrypted.Xor(keyArray).CopyTo(decrypted, 0);
            return Encoding.UTF8.GetString(decrypted);
        }
    }
}
