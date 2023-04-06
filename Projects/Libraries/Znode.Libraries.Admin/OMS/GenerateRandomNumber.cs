using System;

namespace Znode.Libraries.Admin
{
    public static class GenerateRandomNumber
    {
        /// <summary>
        /// Get the next random number for gift card.
        /// </summary>
        /// <returns>Returns the unique gift card number.</returns>
        public static string GetNextGiftCardNumber()
        {
            System.Security.Cryptography.RandomNumberGenerator numberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
            int numberLength = 10;
            char[] chars = new char[numberLength];

            string validChars = "ABCEDFGHIJKLMNOPQRSTUVWXYZ1234567890";

            for (int i = 0; i < numberLength; i++)
            {
                byte[] bytes = new byte[1];
                numberGenerator.GetBytes(bytes);
                Random rnd = new Random(bytes[0]);
                chars[i] = validChars[rnd.Next(0, 35)];
            }

            return new string(chars);
        }
    }
}
