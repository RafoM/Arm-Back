using System.Security.Cryptography;
using System.Text;

namespace IdentityService.Common.Helpers
{
    public static class ReferralHelper
    {
        private static string GetKey()
        {
            var key = Environment.GetEnvironmentVariable("REFERRAL_ENCRYPTION_KEY");

            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Referral encryption key not found in environment variables.");

            if (key.Length != 32)
                throw new InvalidOperationException("Referral encryption key must be 32 characters long (256-bit).");

            return key;
        }

        public static string EncryptUserId(Guid userId)
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(GetKey());
            aes.GenerateIV();

            var encryptor = aes.CreateEncryptor();
            var plainBytes = Encoding.UTF8.GetBytes(userId.ToString());
            var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

            var result = Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
            return result.Replace('+', '-').Replace('/', '_'); 
        }

        public static Guid DecryptReferralCode(string encrypted)
        {
            encrypted = encrypted.Replace('-', '+').Replace('_', '/');
            var fullBytes = Convert.FromBase64String(encrypted);
            var iv = fullBytes.Take(16).ToArray();
            var cipherBytes = fullBytes.Skip(16).ToArray();

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(GetKey());
            aes.IV = iv;

            var decryptor = aes.CreateDecryptor();
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

            var userId = Encoding.UTF8.GetString(plainBytes);
            return Guid.Parse(userId);
        }
    }

}
