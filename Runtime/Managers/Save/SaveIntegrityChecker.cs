using System;
using System.Security.Cryptography;
using System.Text;

namespace MobileFramework.Core.Managers.Save
{
    /// <summary>
    /// Checksum SHA-256 salato del payload di salvataggio. Non è crittografia:
    /// scoraggia l'editing manuale del file, non protegge da attacchi seri.
    /// </summary>
    public static class SaveIntegrityChecker
    {
        private const string Salt = "mfw_core_v1";

        public static string ComputeChecksum(string content)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(content + Salt));

            var builder = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        public static bool Verify(string content, string checksum)
        {
            if (string.IsNullOrEmpty(checksum))
            {
                return false;
            }

            return string.Equals(ComputeChecksum(content), checksum, StringComparison.OrdinalIgnoreCase);
        }
    }
}
