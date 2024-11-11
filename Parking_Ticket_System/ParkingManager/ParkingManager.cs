using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ParkingTicketSystem.Utils;

namespace sampleLang
{
    public class ParkingManager
    {
        // These are our tools and storage
        private FileHandler fileHandler;        // read/write files
        private Validation validator;           // check if user input is correct
        private ParkingConfig parkingConfig;    // Stores our parking settings
        private List<ParkingSlot> parkingSlots; // List of all parking slots
        private readonly ParkingTransaction _parkingTransaction;

        public ParkingManager()
        {
            fileHandler = new FileHandler();
            validator = new Validation();
            parkingSlots = new List<ParkingSlot>();
            _parkingTransaction = new ParkingTransaction();
            LoadParking();
        }

        // Try to load existing parking data
        private void LoadParking()
        {
            parkingConfig = fileHandler.LoadParkingData();

            // If we found existing data, create the parking slots
            if (parkingConfig != null)
            {
                CreateParkingSlots();
            }
        }

        // Make all parking slots
        private void CreateParkingSlots()
        {

            parkingSlots.Clear();

            // Calculate total number of slots needed
            int totalSlots = parkingConfig.floors * parkingConfig.spacesPerFloor;

            // Create one slot at a time
            for (int i = 0; i < totalSlots; i++)
            {
                // Calculate which floor this slot belongs to
                int currentFloor = (i / parkingConfig.spacesPerFloor) + 1;

                // Calculate space number on that floor
                int spaceNumber = (i % parkingConfig.spacesPerFloor) + 1;

                // Create and add the new slot
                parkingSlots.Add(new ParkingSlot(currentFloor, spaceNumber));
            }
        }

        // Show the main menu
        public void ShowParkingMenu()
        {
            bool running = true;
            while (running)
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
                        running = false;
                        break;
                }
            }
        }


        private void InitializeParking()
        {
            Console.WriteLine("\n=== Initialize Parking ===");

            // Get parking information from user
            int floors = validator.IntValidation("Enter number of floors: ", 1, 10);
            int spaces = validator.IntValidation("Enter number of spaces per floor: ", 1, 50);
            decimal rate = decimal.Parse(validator.stringValidation("Enter hourly parking rate: ", 1, 10));


            parkingConfig = new ParkingConfig(floors, spaces, rate);

            // Create all parking slots
            CreateParkingSlots();

            // Save everything to file
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);

            Console.WriteLine("\nParking information initialized successfully!");
            ShowParkingMenu();
        }

        // Change the parking fee
        private void ChangeParkingFee()
        {
            Console.WriteLine("\n=== Change Parking Fee ===");

            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                ShowParkingMenu();
                return;
            }

            // Show current rate and get new rate
            Console.WriteLine($"Current hourly rate: {parkingConfig.hourlyRate}");
            decimal newRate = decimal.Parse(validator.stringValidation("Enter new hourly parking rate: ", 1, 10));

            // Update and save
            parkingConfig.hourlyRate = newRate;
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);

            Console.WriteLine("Parking fee updated successfully!");
            ShowParkingMenu();
        }

        // Change status of a parking slot
        private void ChangeSlotStatus()
        {
            Console.WriteLine("\n=== Change Slot Status ===");

            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                ShowParkingMenu();
                return;
            }

            // Get slot information
            string floor = validator.stringValidation("Enter floor number (e.g., 01): ", 2, 2);
            string slot = validator.stringValidation("Enter slot number (e.g., 01): ", 2, 2);
            string slotId = $"F{floor}S{slot}";

            try
            {
                // Read the parking.txt file to check current status
                string[] lines = System.IO.File.ReadAllLines("Parking.txt");

                // Skip the first 3 configuration lines
                for (int i = 3; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('|');
                    if (parts[0] == slotId)
                    {
                        // Check if the slot is occupied
                        if (parts[1].Contains("OCCUPIED"))
                        {
                            Console.WriteLine("\nError: Cannot modify an occupied parking slot!");
                            ShowParkingMenu();
                            return;
                        }
                        break;
                    }
                }

                // Find the slot in our list
                ParkingSlot parkingSlot = null;
                foreach (ParkingSlot ps in parkingSlots)
                {
                    if (ps.id == slotId)
                    {
                        parkingSlot = ps;
                        break;
                    }
                }

                // Check if slot exists
                if (parkingSlot == null)
                {
                    Console.WriteLine("Error: Invalid parking slot.");
                    ShowParkingMenu();
                    return;
                }

                // Show current status and get new status
                Console.WriteLine($"\nCurrent status: {parkingSlot.status}");
                Console.WriteLine("\nSelect new status:");
                Console.WriteLine("1 - Available");
                Console.WriteLine("2 - Not Available");
                Console.WriteLine("3 - Under Maintenance");

                int statusChoice = validator.IntValidation("Enter choice: ", 1, 3);

                // Update status based on choice
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

                // Save changes
                fileHandler.SaveParkingData(parkingConfig, parkingSlots);
                Console.WriteLine($"\nSlot status updated to {parkingSlot.status}");
                ShowParkingMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                ShowParkingMenu();
            }
        }
    }
}
