using System;
using System.Collections.Generic;
using System.IO;

namespace sampleLang
{
    public class FileHandler
    {
        // The name of our parking data file
        private string fileName = "Parking.txt";

        // Method to check if our parking file exists
        public bool DoesParkingFileExist()
        {
            return File.Exists(fileName);
        }

        // Method to save all parking information to file
        public void SaveParkingData(ParkingConfig config, List<ParkingSlot> slots)
        {
            try
            {
                // Create a new file (or overwrite if it exists)
                StreamWriter fileWriter = new StreamWriter(fileName);

                // First, save the basic parking configuration
                fileWriter.WriteLine("Floors|" + config.floors);
                fileWriter.WriteLine("SpacesPerFloor|" + config.spacesPerFloor);
                fileWriter.WriteLine("HourlyRate|" + config.hourlyRate);

                // Then save information about each parking slot
                foreach (ParkingSlot slot in slots)
                {
                    // Save in format: "F01S01|AVAILABLE"
                    fileWriter.WriteLine(slot.id + "|" + slot.status);
                }

                // Close the file when we're done
                fileWriter.Close();

                Console.WriteLine("Parking data saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving parking data: " + ex.Message);
            }
        }

        // Method to change only the hourly rate
        public void ChangeHourlyRate(decimal newRate)
        {
            try
            {
                // Read all lines from file
                string[] allLines = File.ReadAllLines(fileName);

                // Go through each line
                for (int i = 0; i < allLines.Length; i++)
                {
                    // If we find the hourly rate line
                    if (allLines[i].StartsWith("HourlyRate"))
                    {
                        // Update it with the new rate
                        allLines[i] = "HourlyRate|" + newRate;
                        break;
                    }
                }

                // Save all lines back to file
                File.WriteAllLines(fileName, allLines);
                Console.WriteLine("Hourly rate updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating hourly rate: " + ex.Message);
            }
        }

        // Method to load parking configuration from file
        public ParkingConfig LoadParkingData()
        {
            try
            {
                // Check if file exists
                if (!DoesParkingFileExist())
                {
                    Console.WriteLine("No parking configuration found.");
                    return null;
                }

                // Read all lines from file
                string[] allLines = File.ReadAllLines(fileName);

                // Variables to store our configuration
                int floors = 0;
                int spaces = 0;
                decimal rate = 0;

                // Go through each line
                foreach (string line in allLines)
                {
                    // Split the line into parts (e.g., "Floors|5" becomes ["Floors", "5"])
                    string[] parts = line.Split('|');

                    // Check what type of configuration this line contains
                    if (parts[0] == "Floors")
                    {
                        floors = int.Parse(parts[1]);
                    }
                    else if (parts[0] == "SpacesPerFloor")
                    {
                        spaces = int.Parse(parts[1]);
                    }
                    else if (parts[0] == "HourlyRate")
                    {
                        rate = decimal.Parse(parts[1]);
                    }
                }

                // Create and return new configuration with loaded values
                return new ParkingConfig(floors, spaces, rate);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading parking data: " + ex.Message);
                return null;
            }
        }
    }
}