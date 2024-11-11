namespace sampleLang

{
    public class ParkingSlot
    {
        // Basic information about the parking slot
        public string id;              // Unique identifier for the slot (e.g., F01S01)
        public string status;          // Current status (AVAILABLE/NOT AVAILABLE/UNDER MAINTENANCE)
        public int floorNumber;        // Floor where slot is located
        public int spaceNumber;        // Space number on the floor

        // Constructor to initialize a new parking slot
        public ParkingSlot(int floor, int space)
        {
            floorNumber = floor;
            spaceNumber = space;
            id = $"F{floor:D2}S{space:D2}";  // Creates ID in format F01S01
            status = "AVAILABLE";             // New slots are always available initially
        }
    }
}