 using System;
using System.IO;

namespace sampleLang
{
    public class ParkingTransaction
    {
        public static readonly string PARKING_FILE = Path.Combine("Parking.txt");
        private readonly FileHandler _fileHandler = new FileHandler();

        public static void PrintFloorStats(int floor, int available, int occupied)
        {
            Console.WriteLine($"\nFloor {floor}:");
            Console.WriteLine($"Available Slots: {available}");
            Console.WriteLine($"Occupied Slots: {occupied}");
        }

        public void ParkVehicle()
        {
            Console.WriteLine("\n=== Park Vehicle ===");
            
            try
            {
                // Read all lines first
                string[] allLines = File.ReadAllLines(PARKING_FILE);
                bool foundSpot = false;
                int lineIndex = 3; // Skip configuration lines

                Console.Write("Enter Vehicle Plate Number: ");
                string plateNumber = Console.ReadLine();

                for (int i = lineIndex; i < allLines.Length; i++)
                {
                    string[] parts = allLines[i].Split('|');
                    if (parts[1] == "AVAILABLE")
                    {
                        // Found an available spot
                        allLines[i] = $"{parts[0]}|OCCUPIED|{plateNumber}|{DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                        foundSpot = true;
                        
                        Console.WriteLine($"\nVehicle successfully parked!");
                        Console.WriteLine($"Slot Number: {parts[0]}");
                        Console.WriteLine($"Entry Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        break;
                    }
                }

                if (foundSpot)
                {
                    File.WriteAllLines(PARKING_FILE, allLines);
                }
                else
                {
                    Console.WriteLine("Sorry, no parking slots available.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing parking entry: {ex.Message}");
            }
        }

        public void ExitVehicle()
        {
            Console.WriteLine("\n=== Vehicle Exit ===");
            
            try
            {
                Console.Write("Enter Vehicle Plate Number: ");
                string plateNumber = Console.ReadLine();

                string[] allLines = File.ReadAllLines(PARKING_FILE);
                bool foundVehicle = false;
                decimal hourlyRate = 0;

                // Get hourly rate from configuration
                string[] rateLine = allLines[2].Split('|');
                if (rateLine[0] == "HourlyRate")
                {
                    hourlyRate = decimal.Parse(rateLine[1]);
                }

                for (int i = 3; i < allLines.Length; i++)
                {
                    string[] parts = allLines[i].Split('|');
                    if (parts.Length >= 4 && parts[2] == plateNumber)
                    {
                        // Found the vehicle
                        DateTime entryTime = DateTime.Parse(parts[3]);
                        DateTime exitTime = DateTime.Now;
                        TimeSpan duration = exitTime - entryTime;
                        
                        // Calculate fee
                        decimal totalHours = (decimal)Math.Ceiling(duration.TotalHours);
                        decimal fee = totalHours * hourlyRate;

                        // Update parking slot to available
                        allLines[i] = $"{parts[0]}|AVAILABLE";
                        
                        Console.WriteLine("\nExit Details:");
                        Console.WriteLine($"Duration: {duration.Hours}h {duration.Minutes}m");
                        Console.WriteLine($"Parking Fee: PHP {fee:N2}");

                        foundVehicle = true;
                        break;
                    }
                }

                if (foundVehicle)
                {
                    File.WriteAllLines(PARKING_FILE, allLines);
                }
                else
                {
                    Console.WriteLine("Vehicle not found in parking.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing vehicle exit: {ex.Message}");
            }
        }
    }
}