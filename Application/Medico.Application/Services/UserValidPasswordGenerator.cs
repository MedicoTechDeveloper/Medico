using System;
using System.Collections.Generic;
using Medico.Application.Interfaces;

namespace Medico.Application.Services
{
    public class UserValidPasswordGenerator : IUserValidPasswordGenerator
    {
        private readonly char[] _lowerCaseChars =
        {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'u', 'r', 's', 'u',
            'v', 'w', 'x', 'y', 'z'
        };

        private readonly char[] _upperCaseChars =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
            'V', 'W', 'X', 'Y', 'Z'
        };

        private readonly char[] _digitChars =
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9'
        };

        private readonly char[] _nonAlphanumericChars =
        {
            '!', '@', '$', '%', '^', '&', '*', '(', ')', '#'
        };

        public string Generate()
        {
            var random = new Random();

            var lowerCaseChar = GetRandomChar(random, _lowerCaseChars);
            var upperCaseChar = GetRandomChar(random, _upperCaseChars);
            var digitChar = GetRandomChar(random, _digitChars);
            var nonAlphanumericChar = GetRandomChar(random, _nonAlphanumericChars);

            var passwordChars = new[]
            {
                lowerCaseChar,
                upperCaseChar,
                digitChar,
                nonAlphanumericChar,
                GetRandomValidPasswordChar(random),
                GetRandomValidPasswordChar(random)
            };

            return new string(passwordChars);
        }

        private static char GetRandomChar(Random random, IReadOnlyList<char> chars)
        {
            return chars[random.Next(chars.Count)];
        }

        private char GetRandomValidPasswordChar(Random random)
        {
            var charType = random.Next(1, 5);
            switch (charType)
            {
                case 1:
                    return GetRandomChar(random, _lowerCaseChars);
                case 2:
                    return GetRandomChar(random, _upperCaseChars);
                case 3:
                    return GetRandomChar(random, _digitChars);
                case 4:
                    return GetRandomChar(random, _nonAlphanumericChars);
            }

            throw new InvalidOperationException();
        }
    }
}