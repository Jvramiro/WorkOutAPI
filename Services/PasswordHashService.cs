using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace WorkOutAPI.Services
{
    public class PasswordHashService
    {
        private const int SaltSize = 16;
        private const int HashSize = 16;
        private const int Iterations = 6;
        private const int MemorySize = 65536;

        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            int parallelism = Math.Max(1, Environment.ProcessorCount / 2);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = Iterations,
                MemorySize = MemorySize,
                DegreeOfParallelism = parallelism
            };

            byte[] hash = argon2.GetBytes(HashSize);

            string hashStr = string.Join("$", "argon2id", Iterations, MemorySize, parallelism, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
            return hashStr;
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('$');

            if(parts.Length != 6 || parts[0] != "argon2id")
                return false;

            int Iterations = int.Parse(parts[1]);
            int MemorySize = int.Parse(parts[2]);
            int parallelism = int.Parse(parts[3]);
            byte[] salt = Convert.FromBase64String(parts[4]);
            byte[] hash = Convert.FromBase64String(parts[5]);

            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                Iterations = Iterations,
                MemorySize = MemorySize,
                DegreeOfParallelism = parallelism
            };

            byte[] computedHash = argon2.GetBytes(hash.Length);
            bool isValid = CryptographicOperations.FixedTimeEquals(hash, computedHash);

            return isValid;
        }
    }
}