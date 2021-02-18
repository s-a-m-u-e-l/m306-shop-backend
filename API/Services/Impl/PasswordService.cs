using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace API.Services.Impl
{
    public class PasswordService : IPasswordService
    {
        private const int SALT_BITS = 256;
        private const char SALT_SPLIT_CHAR = '$';
        private const int ITERATIONS = 10000;
        private const int KEY_BYTES = 256 / 8;

        private const KeyDerivationPrf ALGORITHM = KeyDerivationPrf.HMACSHA512;

        public string HashPassword(string password, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password might not be null or zero length.");
            }

            // Generate salt if none has been provided
            if (salt == null)
            {
                byte[] bytes = new byte[256 / 8];

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }

                salt = bytes;
            }

            var key = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: ALGORITHM,
                iterationCount: ITERATIONS,
                numBytesRequested: KEY_BYTES);

            var hash = $"{Convert.ToBase64String(salt)}{SALT_SPLIT_CHAR}{Convert.ToBase64String(key)}";
            return hash;
        }

        public bool CheckPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password might not be null or zero length.");
            }

            if (string.IsNullOrEmpty(passwordHash))
            {
                throw new ArgumentException("PasswordHash might not ne bull or zero length.");
            }

            var split = passwordHash.Split(SALT_SPLIT_CHAR);

            var hash = HashPassword(password, Convert.FromBase64String(split[0]));
            return hash.Equals(passwordHash);
        }
    }
}