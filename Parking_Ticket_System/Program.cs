using System;
using ParkingTicketSystem.Models;
using ParkingTicketSystem.Utils;
using sampleLang;

namespace ParkingTicketSystem
{
    internal class Program
    {
        private static readonly Validation Validator = new Validation();
        private static readonly Account AccountManager = new Account();
        private static readonly ParkingManager ParkingManager = new ParkingManager();

        static void Main(string[] args)
        {
            DisplayWelcomeMessage();
            while (true)
            {
                DisplayMainMenu();
                int choice = Validator.IntValidation("Enter your choice: ", 1, 3);
                ProcessMenuChoice(choice);
            }
        }

        static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to Parking Management System");
            Console.WriteLine("===================================");
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("\nMain Menu");
            Console.WriteLine("1. Account Management");
            Console.WriteLine("2. Parking Management");
            Console.WriteLine("3. Exit");
        }

        static void ProcessMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    AccountManager.systemMenu();
                    break;
                case 2:
                    ParkingManager.ShowParkingMenu();
                    break;
                case 3:
                    ExitProgram();
                    break;
            }
        }

        static void ExitProgram()
        {
            Console.WriteLine("\nThank you for using Parking Management System!");
            Environment.Exit(0);
        }
    }
}