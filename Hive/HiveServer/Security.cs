using System.Security.Cryptography;
using System.Text;

namespace HiveServer
{
    public class Security
    {
        private static int s_saltSize = 16;
        private static int s_authTokentSize = 32;

        public static string GetRandomSalt() 
        {
            byte[] newSaltBuffer = new byte[s_saltSize];
            Random random = new Random();

            random.NextBytes(newSaltBuffer);

            return Encoding.Default.GetString(newSaltBuffer);
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
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[s_authTokentSize];
            byte[] randomBytes = new byte[s_authTokentSize];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < s_authTokentSize; i++)
            {
                chars[i] = allowedChars[randomBytes[i] % allowedChars.Length];
            }

            return new string(chars);
        }
    }
}
