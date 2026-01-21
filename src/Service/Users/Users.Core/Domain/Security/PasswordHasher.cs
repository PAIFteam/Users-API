using System.Security.Cryptography;

namespace Users.Core.Domain.Security
{
    public static class PasswordHasher
    {
        private const int KeySize = 32;
        private const int Iterations = 100_000;

        public static string HashPassword(string password, string globalSalt)
        {
            var saltBytes = GetSaltBytes(globalSalt);
            var derived = HashPasswordInternal(password, saltBytes);
            var digest = SHA256.HashData(derived);
            return Convert.ToHexString(digest)[..32];
        }

        public static bool VerifyPassword(string password, string storedHashBase64, string globalSalt)
        {
            if (string.IsNullOrWhiteSpace(storedHashBase64) || string.IsNullOrWhiteSpace(globalSalt))
                return false;

            var storedHash = System.Text.Encoding.ASCII.GetBytes(storedHashBase64);

            var saltBytes = GetSaltBytes(globalSalt);
            var derived = HashPasswordInternal(password, saltBytes);
            var digest = SHA256.HashData(derived);
            var computedHex = Convert.ToHexString(digest)[..32];
            var computedBytes = System.Text.Encoding.ASCII.GetBytes(computedHex);
            return CryptographicOperations.FixedTimeEquals(storedHash, computedBytes);
        }

        private static byte[] HashPasswordInternal(string password, byte[] salt)
        {
            return Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);
        }

        private static byte[] GetSaltBytes(string globalSalt)
        {
            return SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(globalSalt));
        }
    }
}
