using System;
using System.Text;

namespace Medico.Application.Extensions
{
    public static class StringExtensions
    {
        public static string Encrypt(this string password)
        {
            return string.IsNullOrEmpty(password)
                ? password
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
        }

        public static string Decrypt(this string passwordHash)
        {
            return string.IsNullOrEmpty(passwordHash)
                ? passwordHash
                : Encoding.UTF8.GetString(Convert.FromBase64String(passwordHash));
        }
    }
}