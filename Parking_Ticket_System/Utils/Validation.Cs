using System;

namespace ParkingTicketSystem.Utils
{
    public class Validation
    {
        public int IntValidation(string message, int minSize, int maxSize)
        {
            int nValue;
            bool isValid = false;
            do
            {
                Console.Write(message);
                string input = Console.ReadLine();
                if (int.TryParse(input, out nValue) && nValue >= minSize && nValue <= maxSize)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine("Invalid Choice! Please try again.");
                }
            } while (!isValid);
            return nValue;
        }

        public string stringValidation(string message, int minSize, int maxSize)
        {
            string nValue;
            do
            {
                Console.Write(message);
                nValue = Console.ReadLine();
                if (nValue.Length < minSize || nValue.Length > maxSize)
                {
                    Console.WriteLine($"Input must be {minSize} to {maxSize} characters! Please try again.");
                }
            } while (nValue.Length < minSize || nValue.Length > maxSize);
            return nValue;
        }

        public string deptValidation(string message)
        {
            string nValue;
            do
            {
                Console.Write(message);
                nValue = Console.ReadLine().ToUpper();
                if (nValue != "M" && nValue != "S")
                {
                    Console.WriteLine($"Input must be [M] Manager or [S] Staff! Please try again.");
                }
            } while (nValue != "M" && nValue != "S");
            return nValue;
        }
    }
} 