using VehicleService.Domain.Enums;

namespace VehicleService.Domain.Models
{
    public class Vehicle
    {
        public Guid Id { get; private set; }
        public string RegistrationNumber { get; private set; }
        public string Model { get; private set; }
        public string Manufacturer { get; private set; }
        public int Year { get; private set; }
        public string VIN { get; private set; }
        public VehicleType Type { get; private set; }
        public float FuelCapacity { get; private set; }
        public float CurrentFuelLevel { get; private set; }
        public float OdometerReading { get; private set; }
        public VehicleStatus Status { get; private set; }
        public Guid? AssignedDriverId { get; private set; }
        public VehicleLocation LastKnownLocation { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public List<MaintenanceRecord> MaintenanceHistory { get; private set; } = new List<MaintenanceRecord>();
        public List<FuelRecord> FuelHistory { get; private set; } = new List<FuelRecord>();

        private Vehicle() { }

        public static Vehicle Create(
            string registrationNumber,
            string model,
            string manufacturer,
            int year,
            string vin,
            VehicleType type,
            float fuelCapacity,
            float currentFuelLevel,
            float odometerReading)
        {
            var vehicle = new Vehicle
            {
                Id = Guid.NewGuid(),
                RegistrationNumber = registrationNumber,
                Model = model,
                Manufacturer = manufacturer,
                Year = year,
                VIN = vin,
                Type = type,
                FuelCapacity = fuelCapacity,
                CurrentFuelLevel = currentFuelLevel,
                OdometerReading = odometerReading,
                Status = VehicleStatus.ACTIVE,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return vehicle;
        }

        public void Update(
            string registrationNumber,
            string model,
            string manufacturer,
            int year,
            string vin,
            VehicleType type,
            float fuelCapacity,
            float currentFuelLevel,
            float odometerReading,
            VehicleStatus status)
        {
            RegistrationNumber = registrationNumber;
            Model = model;
            Manufacturer = manufacturer;
            Year = year;
            VIN = vin;
            Type = type;
            FuelCapacity = fuelCapacity;
            CurrentFuelLevel = currentFuelLevel;
            OdometerReading = odometerReading;
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AssignDriver(Guid driverId, DateTime? endDate = null)
        {
            AssignedDriverId = driverId;
            Status = VehicleStatus.RESERVED;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UnassignDriver()
        {
            AssignedDriverId = null;
            Status = VehicleStatus.ACTIVE;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLocation(double latitude, double longitude, double speed, double heading)
        {
            LastKnownLocation = new VehicleLocation
            {
                Latitude = latitude,
                Longitude = longitude,
                Speed = speed,
                Heading = heading,
                Timestamp = DateTime.UtcNow
            };
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetStatus(VehicleStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddFuel(float amount, float cost, string station, string driver)
        {
            FuelHistory.Add(new FuelRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = Id,
                Amount = amount,
                Cost = cost,
                Station = station,
                DriverName = driver,
                OdometerReading = OdometerReading,
                Timestamp = DateTime.UtcNow
            });

            CurrentFuelLevel += amount;
            if (CurrentFuelLevel > FuelCapacity)
            {
                CurrentFuelLevel = FuelCapacity;
            }

            UpdatedAt = DateTime.UtcNow;
        }

        public void AddMaintenanceRecord(string description, string technician, float cost, DateTime completedDate)
        {
            MaintenanceHistory.Add(new MaintenanceRecord
            {
                Id = Guid.NewGuid(),
                VehicleId = Id,
                Description = description,
                Technician = technician,
                Cost = cost,
                OdometerReading = OdometerReading,
                CompletedDate = completedDate,
                CreatedAt = DateTime.UtcNow
            });

            UpdatedAt = DateTime.UtcNow;
        }
    }
}
