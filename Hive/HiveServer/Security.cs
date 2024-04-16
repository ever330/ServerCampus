using System.Security.Cryptography;
using System.Text;

namespace HiveServer
{
    public class Security
    {
        private const int SaltSize = 16;
        private const int AuthTokentSize = 32;
        private const string AllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GetRandomSalt()
        {
            char[] chars = new char[SaltSize];
            byte[] randomBytes = new byte[SaltSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < SaltSize; i++)
            {
                chars[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
            }

            return new string(chars);
        }

        public static string Hasing(string password, string salt)
        {
            string encryptPassword;
            using (SHA256 sha256 = SHA256.Create())
            {
                StringBuilder hash = new StringBuilder();
                byte[] hashArray = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                foreach (byte b in hashArray)
                {
                    hash.Append(b.ToString("x"));
                }
                encryptPassword = hash.ToString();
            }

            return encryptPassword;
        }

        public static string CreateAuthToken()
        {
            char[] chars = new char[AuthTokentSize];
            byte[] randomBytes = new byte[AuthTokentSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < AuthTokentSize; i++)
            {
                chars[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
            }

            return new string(chars);
        }
    }
}
