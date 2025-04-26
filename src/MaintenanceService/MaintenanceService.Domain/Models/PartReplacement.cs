namespace MaintenanceService.Domain.Models
{
    public class PartReplacement
    {
        public Guid Id { get; private set; }
        public string PartId { get; private set; }
        public string PartName { get; private set; }
        public int Quantity { get; private set; }
        public double Cost { get; private set; }

        public PartReplacement(string partId, string partName, int quantity, double cost)
        {
            Id = Guid.NewGuid();
            PartId = partId;
            PartName = partName;
            Quantity = quantity;
            Cost = cost;
        }
    }
}
