using AutoMapper;
using MaintenanceService.API.Protos;
using MaintenanceService.Domain.Models;

namespace MaintenanceService.API.Mapping
{
    public class MaintenanceProfile : Profile
    {
        public MaintenanceProfile()
        {
            CreateMap<Domain.Models.MaintenanceTask, Protos.MaintenanceTask>()
                .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId.ToString()))
                .ForMember(dest => dest.TaskDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (MaintenanceType)(int)src.Type))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src =>
                    src.ScheduledDate != default ? (ulong)new DateTimeOffset(src.ScheduledDate).ToUnixTimeSeconds() : 0))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (MaintenanceStatus)(int)src.Status))
                .ForMember(dest => dest.AssignedTechnicianId, opt => opt.MapFrom(src =>
                    src.AssignedTechnicianId.HasValue ? src.AssignedTechnicianId.Value.ToString() : string.Empty))
                .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
            .ForMember(dest => dest.RequiredParts, opt => opt.MapFrom(src => src.RequiredParts));

            CreateMap<MaintenanceScheduleResponse, Domain.Models.MaintenanceTask>();
            CreateMap<MaintenanceScheduleResponse, Domain.Models.MaintenanceTask>().ReverseMap();

            CreateMap<Protos.MaintenanceTask, Domain.Models.MaintenanceTask>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.Parse(src.TaskId)))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => Guid.Parse(src.VehicleId)))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.TaskDescription))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (Domain.Enums.MaintenanceType)(int)src.Type))
                .ForMember(dest => dest.ScheduledDate, opt => opt.MapFrom(src =>
                    src.ScheduledDate != 0 ? DateTimeOffset.FromUnixTimeSeconds((long)src.ScheduledDate).DateTime : default))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (Domain.Enums.MaintenanceStatus)(int)src.Status))
                .ForMember(dest => dest.AssignedTechnicianId, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.AssignedTechnicianId) ? (Guid?)null : Guid.Parse(src.AssignedTechnicianId)))
                .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
                .ForMember(dest => dest.RequiredParts, opt => opt.MapFrom(src => src.RequiredParts))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // Set in constructor
                .ForMember(dest => dest.CompletedAt, opt => opt.Ignore()) // Set in domain logic
                .ForMember(dest => dest.DomainEvents, opt => opt.Ignore()); // Handled by domain logic

            CreateMap<Domain.Models.RequiredPart, Protos.RequiredPart>()
                .ForMember(dest => dest.PartId, opt => opt.MapFrom(src => src.PartId))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PartName, opt => opt.MapFrom(src => src.PartName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            //CreateMap<Domain.Models.RequiredPart, RequiredPart>()
            //    .ForMember(dest => dest.PartId, opt => opt.MapFrom(src => src.PartId))
            //    .ForMember(dest => dest.PartName, opt => opt.MapFrom(src => src.PartName))
            //    .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            //    .ReverseMap();

            // MaintenanceEvent mappings
            CreateMap<Domain.Models.MaintenanceEvent, Protos.MaintenanceEvent>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId.ToString()))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (MaintenanceType)(int)src.Type))
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src =>
                    src.Timestamp != default ? (ulong)new DateTimeOffset(src.Timestamp).ToUnixTimeSeconds() : 0))
                .ForMember(dest => dest.PerformedBy, opt => opt.MapFrom(src => src.PerformedBy))
                .ForMember(dest => dest.OdometerReading, opt => opt.MapFrom(src => src.OdometerReading))
                .ForMember(dest => dest.PartsReplaced, opt => opt.MapFrom(src => src.PartsReplaced))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<Domain.Models.MaintenanceEvent, Protos.MaintenanceEvent>()
                 .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.Id.ToString()))
                 .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId.ToString()))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                 .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (MaintenanceType)(int)src.Type))
                 .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src =>
                     src.Timestamp != default ? (ulong)new DateTimeOffset(src.Timestamp).ToUnixTimeSeconds() : 0))
                 .ForMember(dest => dest.PerformedBy, opt => opt.MapFrom(src => src.PerformedBy))
                 .ForMember(dest => dest.OdometerReading, opt => opt.MapFrom(src => src.OdometerReading))
                 .ForMember(dest => dest.PartsReplaced, opt => opt.MapFrom(src => src.PartsReplaced))
                 .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                 .ReverseMap();

            // PartReplacement mappings
            CreateMap<Domain.Models.PartReplacement, Protos.PartReplacement>()
                .ForMember(dest => dest.PartId, opt => opt.MapFrom(src => src.PartId))
                .ForMember(dest => dest.PartName, opt => opt.MapFrom(src => src.PartName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost));

            CreateMap<Domain.Models.PartReplacement, Protos.PartReplacement>()
                .ForMember(dest => dest.PartId, opt => opt.MapFrom(src => src.PartId))
                .ForMember(dest => dest.PartName, opt => opt.MapFrom(src => src.PartName))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ReverseMap();


            CreateMap<VehicleHealthMetrics, VehicleHealthMetricsResponse>()
                .ForMember(dest => dest.VehicleId, opt => opt.MapFrom(src => src.VehicleId.ToString()))
                .ForMember(dest => dest.OverallHealthScore, opt => opt.MapFrom(src => src.OverallHealthScore))
                .ForMember(dest => dest.LastMaintenanceDate, opt => opt.MapFrom(src =>
                    src.LastMaintenanceDate.HasValue ? (ulong)new DateTimeOffset(src.LastMaintenanceDate.Value).ToUnixTimeSeconds() : 0))
                .ForMember(dest => dest.NextMaintenanceDue, opt => opt.MapFrom(src =>
                    src.NextMaintenanceDue.HasValue ? (ulong)new DateTimeOffset(src.NextMaintenanceDue.Value).ToUnixTimeSeconds() : 0))
                .ForMember(dest => dest.MaintenanceEventsCount, opt => opt.MapFrom(src => src.MaintenanceEventsCount))
                .ForMember(dest => dest.ComponentHealth, opt => opt.MapFrom(src => src.ComponentHealths));

            CreateMap<ComponentHealthMetric, ComponentHealth>()
                .ForMember(dest => dest.ComponentName, opt => opt.MapFrom(src => src.ComponentName))
                .ForMember(dest => dest.HealthScore, opt => opt.MapFrom(src => src.HealthScore))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.LastChecked, opt => opt.MapFrom(src =>
                    src.LastChecked != default ? (ulong)new DateTimeOffset(src.LastChecked).ToUnixTimeSeconds() : 0));
        }
    }
}
