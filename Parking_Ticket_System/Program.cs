using System;
using ParkingTicketSystem.Models;
using ParkingTicketSystem.Utils;
using sampleLang;

namespace ParkingTicketSystem
{
    internal class Program
    {
        private static readonly Account AccountManager = new Account();
        static void Main(string[] args)
        {
            DisplayWelcomeMessage();
            DisplayMainMenu();
        }

        static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Welcome to Parking Management System");
            Console.WriteLine("===================================");
        }

        static void DisplayMainMenu()
        {
            AccountManager.systemMenu();
        }
    }
}