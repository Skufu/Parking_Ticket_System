using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using ParkingTicketSystem.Utils;

namespace sampleLang
{
    // This class handles everything related to managing a parking lot
    public class ParkingManager
    {
        // These are the tools we need to run the parking system
        private FileHandler fileHandler;              // This helps us save and load data from files
        private Validation validator;                 // This makes sure user inputs are correct
        private ParkingConfig parkingConfig;         // This stores our parking lot settings (like number of floors)
        private List<ParkingSlot> parkingSlots;      // This is our list of all parking spaces
        private readonly ParkingTransaction _parkingTransaction;  // This handles cars entering and leaving

        // This runs when we first start the parking system
        public ParkingManager()
        {
            // Create new instances of all our tools
            fileHandler = new FileHandler();
            validator = new Validation();
            parkingSlots = new List<ParkingSlot>();
            _parkingTransaction = new ParkingTransaction();
            LoadParking(); // Try to load any existing parking data
        }

        // This tries to load any saved parking information from a file
        private void LoadParking()
        {
            parkingConfig = fileHandler.LoadParkingData();
            // If we found saved data, create parking slots based on it
            if (parkingConfig != null)
            {
                CreateParkingSlots();
            }
        }

        // This creates all the parking spaces in our parking lot
        private void CreateParkingSlots()
        {
            // Clear any existing slots first
            parkingSlots.Clear();
            
            // Calculate how many total parking spaces we need
            int totalSlots = parkingConfig.floors * parkingConfig.spacesPerFloor;

            // Create each parking slot one by one
            for (int i = 0; i < totalSlots; i++)
            {
                // Math to figure out which floor and space number this slot should be
                // For example: if we have 5 spaces per floor, slot 7 would be on floor 2, space 2
                int currentFloor = (i / parkingConfig.spacesPerFloor) + 1;
                int spaceNumber = (i % parkingConfig.spacesPerFloor) + 1;
                parkingSlots.Add(new ParkingSlot(currentFloor, spaceNumber));
            }
        }

        // This shows the main menu and handles user choices
        public void ShowParkingMenu()
        {
            while (true) // Keep showing the menu until user chooses to exit
            {
                // Show all the options available
                Console.WriteLine("\n=== Parking Management Menu ===");
                Console.WriteLine("1 - Initialize Parking Information");
                Console.WriteLine("2 - Change Parking Fee");
                Console.WriteLine("3 - Change Parking Slot Status");
                Console.WriteLine("4 - View Parking Status");
                Console.WriteLine("5 - Park Vehicle");
                Console.WriteLine("6 - Exit Vehicle");
                Console.WriteLine("7 - Return to Main Menu");

                // Get what the user wants to do
                int choice = validator.IntValidation("\nEnter your choice: ", 1, 7);
                
                // Do what the user asked for
                switch (choice)
                {
                    case 1: // Set up new parking lot
                        InitializeParking();
                        break;
                    case 2: // Change how much we charge
                        ChangeParkingFee();
                        break;
                    case 3: // Change if a spot is available or not
                        ChangeSlotStatus();
                        break;
                    case 4: // Show current parking situation
                        _parkingTransaction.ShowParkingStatus();
                        break;
                    case 5: // Park a new car
                        _parkingTransaction.ParkVehicle();
                        break;
                    case 6: // Remove a parked car
                        _parkingTransaction.ExitVehicle();
                        break;
                    case 7: // Go back to main menu
                        return;
                }
            }
        }

        // This sets up a new parking lot from scratch
        private void InitializeParking()
        {
            Console.WriteLine("\n=== Initialize Parking ===");
            
            // Ask how big the parking lot should be
            int floors = validator.IntValidation("Enter number of floors: ", 1, 10);
            int spaces = validator.IntValidation("Enter number of spaces per floor: ", 1, 50);
            decimal rate = decimal.Parse(validator.stringValidation("Enter hourly parking rate: ", 1, 10));

            // Create the parking lot with these settings
            parkingConfig = new ParkingConfig(floors, spaces, rate);
            CreateParkingSlots();
            
            // Save all this information to a file
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);
            Console.WriteLine("\nParking information initialized successfully!");
        }

        // This lets us change how much we charge for parking
        private void ChangeParkingFee()
        {
            Console.WriteLine("\n=== Change Parking Fee ===");
            
            // Make sure the parking lot is set up first
            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                return;
            }

            // Show current rate and get new rate
            Console.WriteLine($"Current hourly rate: {parkingConfig.hourlyRate}");
            decimal newRate = decimal.Parse(validator.stringValidation("Enter new hourly parking rate: ", 1, 10));
            
            // Save the new rate
            parkingConfig.hourlyRate = newRate;
            fileHandler.SaveParkingData(parkingConfig, parkingSlots);
            Console.WriteLine("Parking fee updated successfully!");
        }

        // This lets us change whether a parking spot is available or not
        private void ChangeSlotStatus()
        {
            Console.WriteLine("\n=== Change Slot Status ===");
            
            // Check if parking lot is set up
            if (parkingConfig == null)
            {
                Console.WriteLine("Error: Parking not initialized. Please initialize first.");
                return;
            }

            // Get which parking spot we want to change
            string floor = validator.stringValidation("Enter floor number (e.g., 01): ", 2, 2);
            string slot = validator.stringValidation("Enter slot number (e.g., 01): ", 2, 2);
            string slotId = $"F{floor}S{slot}"; // Make the slot ID (like "F01S01")

            try
            {
                // Check if this spot is currently being used
                string[] lines = System.IO.File.ReadAllLines("Parking.txt");
                for (int i = 3; i < lines.Length; i++)
                {
                    string[] parts = lines[i].Split('|');
                    if (parts[0] == slotId)
                    {
                        // Can't change status if someone is parked there
                        if (parts[1].Contains("OCCUPIED"))
                        {
                            Console.WriteLine("\nError: Cannot modify an occupied parking slot!");
                            return;
                        }
                        break;
                    }
                }

                // Find this parking spot in our list
                ParkingSlot parkingSlot = null;
                foreach (ParkingSlot ps in parkingSlots)
                {
                    if (ps.id == slotId)
                    {
                        parkingSlot = ps;
                        break;
                    }
                }

                // Make sure we found a valid spot
                if (parkingSlot == null)
                {
                    Console.WriteLine("Error: Invalid parking slot.");
                    return;
                }

                // Show the options for new status
                Console.WriteLine($"\nCurrent status: {parkingSlot.status}");
                Console.WriteLine("\nSelect new status:");
                Console.WriteLine("1 - Available");
                Console.WriteLine("2 - Not Available");
                Console.WriteLine("3 - Under Maintenance");

                // Get and apply the new status
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

                // Save the changes
                fileHandler.SaveParkingData(parkingConfig, parkingSlots);
                Console.WriteLine($"\nSlot status updated to {parkingSlot.status}");
            }
            catch (Exception ex)
            {
                // If anything goes wrong, show the error
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}