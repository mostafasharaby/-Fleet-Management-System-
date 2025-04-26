namespace MaintenanceService.Domain.Events
{
    public abstract class MaintenanceDomainEvent
    {
        public Guid Id { get; }
        public DateTime OccurredOn { get; }

        protected MaintenanceDomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
        }
    }
}
