namespace ProtoR.Application
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.AspNetCore.Identity;

    public static class ExtensionMethods
    {
        public static string ComputeSha256(this string input)
        {
            using var hasher = SHA256.Create();
            var bytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }

        public static string IdentityErrors(this IdentityResult result)
        {
            return string.Join(
                Environment.NewLine,
                result.Errors.Select(e => e.Description));
        }
    }
}
