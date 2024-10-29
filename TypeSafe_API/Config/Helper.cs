using System.Text.Json;
using System.Text;
using BilakLk_API.Models;
using System.Security.Cryptography;
using System;

namespace BilakLk_API.Config
{
    internal class Helper
    {

        #region get LK DateTime
        public DateTime GetLKDateTime()
        {

            DateTime serverTime = DateTime.Now; // gives you current Time in server timeZone
            DateTime utcTime = serverTime.ToUniversalTime(); // convert it to Utc using time zone setting of server computer 

            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time"));
            return localTime;
        }
        #endregion


        internal byte[] EncryptPass(string? clearText)
        {
            if (String.IsNullOrEmpty(clearText))
            {
                return null;
            }
            byte[] encryptedPassword = null;
            string EncryptionKey = "";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                    encryptedPassword = ASCIIEncoding.ASCII.GetBytes(clearText);
                }
            }
            return encryptedPassword;
        }
        internal string EncryptString(string? plainText)
        {
            string key = "E3n9fPc7G2h6T1qA";
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // Initialization vector
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        internal string DecryptString(string? cipherText)
        {
            string key = "E3n9fPc7G2h6T1qA";
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[16]; // Initialization vector
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        internal string GenerateRandomBusinessCode()
        {
            string prefix = "";
            string digits = "0123456789";
            Random random = new Random();

            StringBuilder codeBuilder = new StringBuilder(prefix);
            for (int i = 0; i < 10; i++)
            {
                int randomIndex = random.Next(digits.Length);
                char randomDigit = digits[randomIndex];
                codeBuilder.Append(randomDigit);
            }

            return codeBuilder.ToString();
        }
        internal string GetToken(string email)
        {
            int length = 50;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            StringBuilder sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                sb.Append(chars[index]);
            }
            DateTime currentTime = DateTime.Now;
            string stringKey = sb.ToString();
            string token ="_" + stringKey+"_"+ currentTime.ToString("yyyy-MM-dd HH:mm:ss");
            return token;
        }
        internal string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                stringBuilder.Append(chars[index]);
            }

            return stringBuilder.ToString();
        }
    }

}
