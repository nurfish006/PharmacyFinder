namespace PharmacyFinder.Infrastructure.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }

    public class Argon2PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Using Konscious.Security.Cryptography.Argon2 package
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 1024
            };

            var hash = argon2.GetBytes(32);
            var combined = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combined, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combined, salt.Length, hash.Length);

            return Convert.ToBase64String(combined);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            var combined = Convert.FromBase64String(passwordHash);
            var salt = new byte[16];
            var storedHash = new byte[32];
            
            Buffer.BlockCopy(combined, 0, salt, 0, salt.Length);
            Buffer.BlockCopy(combined, salt.Length, storedHash, 0, storedHash.Length);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 8,
                Iterations = 4,
                MemorySize = 1024 * 1024
            };

            var computedHash = argon2.GetBytes(32);
            return computedHash.SequenceEqual(storedHash);
        }
    }
}