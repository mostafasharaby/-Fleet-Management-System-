namespace MaintenanceService.Domain.Models
{
    public class RequiredPart
    {
        public string Id { get; private set; }
        public string PartId { get; private set; }
        public string PartName { get; private set; }
        public int Quantity { get; private set; }

        public RequiredPart(string id, string partId, string partName, int quantity)
        {
            Id = id;
            PartId = partId;
            PartName = partName;
            Quantity = quantity;
        }
    }
}
