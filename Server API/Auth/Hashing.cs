using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Server_API.Auth
{
    public static class Hashing
    {
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }

        /// <summary>
        /// Hashes the password.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>The hash. It will always be 60 characters of text.</returns>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }

        /// <summary>
        /// Validates the password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="hash">The hash, generated from a previous HashPassword call.</param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Generates a random base64 token of length 64.
        /// </summary>
        /// <returns></returns>
        public static string GenerateToken()
        {
            byte[] random = new byte[48];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(random);
            }
            return Convert.ToBase64String(random);
        }
    }
}