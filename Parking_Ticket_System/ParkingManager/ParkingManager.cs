using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ParkingTicketSystem.Utils;

namespace sampleLang
{
    public class ParkingManager
    {
        private FileHandler fileHandler;
        private Validation validator;
        private ParkingConfig parkingConfig;
        private List<ParkingSlot> parkingSlots;
        private readonly ParkingTransaction _parkingTransaction;

        public ParkingManager()
        {
            fileHandler = new FileHandler();
            validator = new Validation();
            parkingSlots = new List<ParkingSlot>();
            _parkingTransaction = new ParkingTransaction();
            LoadParking();
        }

        private void LoadParking()
        {
            parkingConfig = fileHandler.LoadParkingData();
            if (parkingConfig != null)
            {
                CreateParkingSlots();
            }
        }

        private void CreateParkingSlots()
        {
            parkingSlots.Clear();
            int totalSlots = parkingConfig.floors * parkingConfig.spacesPerFloor;

            for (int i = 0; i < totalSlots; i++)
            {
                int currentFloor = (i / parkingConfig.spacesPerFloor) + 1;
                int spaceNumber = (i % parkingConfig.spacesPerFloor) + 1;
                parkingSlots.Add(new ParkingSlot(currentFloor, spaceNumber));
            }
        }

        public void ShowParkingMenu()
        {
            while (true)
            {
                Console.WriteLine("\n=== Parking Management Menu ===");
                Console.WriteLine("1 - Initialize Parking Information");
                Console.WriteLine("2 - Change Parking Fee");
                Console.WriteLine("3 - Change Parking Slot Status");
                Console.WriteLine("4 - View Parking Status");
                Console.WriteLine("5 - Park Vehicle");
                Console.WriteLine("6 - Exit Vehicle");
                Console.WriteLine("7 - Return to Main Menu");

                int choice = validator.IntValidation("\nEnter your choice: ", 1, 7);
                
                switch (choice)
                {
                    case 1:
                        InitializeParking();
                        break;
                    case 2:
                        ChangeParkingFee();
                        break;
                    case 3:
                        ChangeSlotStatus();
                        break;
                    case 4:
                        _parkingTransaction.ShowParkingStatus();
                        break;
                    case 5:
                        _parkingTransaction.ParkVehicle();
                        break;
                    case 6:
                        _parkingTransaction.ExitVehicle();
                        break;
                    case 7:
                        return;
                }
            }
        }

        private void InitializeParking()
        {
            Console.WriteLine("\n=== Initialize Parking ===");
            int floors = validator.IntValidation("Enter number of floors: ", 1, 10);
            int spaces = validator.IntValidation("Enter number of spaces per floor: ", 1, 50);
            decimal rate = decimal.Parse(validator.stringValidation("Enter hourly parking rate: ", 1, 10));

            parkingConfig = new ParkingConfig(floors, spaces, rate);
            CreateParkingSlots();
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);
            Console.WriteLine("\nParking information initialized successfully!");
        }

        private void ChangeParkingFee()
        {
            Console.WriteLine("\n=== Change Parking Fee ===");
            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                return;
            }

            Console.WriteLine($"Current hourly rate: {parkingConfig.hourlyRate}");
            decimal newRate = decimal.Parse(validator.stringValidation("Enter new hourly parking rate: ", 1, 10));
            parkingConfig.hourlyRate = newRate;
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);
            Console.WriteLine("Parking fee updated successfully!");
        }

        private void ChangeSlotStatus()
        {
            Console.WriteLine("\n=== Change Slot Status ===");
            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                return;
            }

            string floor = validator.stringValidation("Enter floor number (e.g., 01): ", 2, 2);
            string slot = validator.stringValidation("Enter slot number (e.g., 01): ", 2, 2);
            string slotId = $"F{floor}S{slot}";

            try
            {
                string[] lines = System.IO.File.ReadAllLines("Parking.txt");
                for (int i = 3; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('|');
                    if (parts[0] == slotId)
                    {
                        if (parts[1].Contains("OCCUPIED"))
                        {
                            Console.WriteLine("\nError: Cannot modify an occupied parking slot!");
                            return;
                        }
                        break;
                    }
                }

                ParkingSlot parkingSlot = null;
                foreach (ParkingSlot ps in parkingSlots)
                {
                    if (ps.id == slotId)
                    {
                        parkingSlot = ps;
                        break;
                    }
                }

                if (parkingSlot == null)
                {
                    Console.WriteLine("Error: Invalid parking slot.");
                    return;
                }

                Console.WriteLine($"\nCurrent status: {parkingSlot.status}");
                Console.WriteLine("\nSelect new status:");
                Console.WriteLine("1 - Available");
                Console.WriteLine("2 - Not Available");
                Console.WriteLine("3 - Under Maintenance");

                int statusChoice = validator.IntValidation("Enter choice: ", 1, 3);
                switch (statusChoice)
                {
                    case 1:
                        parkingSlot.status = "AVAILABLE";
                        break;
                    case 2:
                        parkingSlot.status = "NOT AVAILABLE";
                        break;
                    case 3:
                        parkingSlot.status = "UNDER MAINTENANCE";
                        break;
                }

                fileHandler.SaveParkingData(parkingConfig, parkingSlots);
                Console.WriteLine($"\nSlot status updated to {parkingSlot.status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}