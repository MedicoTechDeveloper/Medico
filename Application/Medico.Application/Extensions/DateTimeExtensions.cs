using System;

namespace Medico.Application.Extensions
{
    public static class DateTimeExtensions
    {
        public static int GetAge(this DateTime dateOfBirth)
        {
            var today = DateTime.UtcNow;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }
    }
}