namespace sampleLang
{
    public class ParkingConfig
    {
        
        public int floors;             // Total number of floors
        public int spacesPerFloor;     // Number of parking spaces on each floor
        public decimal hourlyRate;     // Cost per hour for parking

        // Constructor to initialize parking configuration
        public ParkingConfig(int totalFloors, int spaces, decimal rate)
        {
            floors = totalFloors;
            spacesPerFloor = spaces;
            hourlyRate = rate;
        }
    }
}