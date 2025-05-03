using FleetManagement.Client.Services;
using RouteService.Domain.Models;

namespace FleetManagement.Client.Workers
{
    internal class RouteWorker : BackgroundService
    {
        private readonly ILogger<RouteWorker> _logger;
        private readonly RouteServiceClient _routeServiceClient;


        public RouteWorker(RouteServiceClient routeServiceClient, ILogger<RouteWorker> logger)
        {
            _routeServiceClient = routeServiceClient;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                //await GetRouteById();
                //await GetAllRoutes();
                //await GetRoutesByVehicle();
                //await GetRoutesByDriver();
                //await GetRoutesByStatus();
                //await GetRoutesByDateRange();
                //await CreateRoute();
                //await UpdateRoute();
                //await DeleteRoute();
                //await OptimizeRoute();
                //await StartRoute();
                //await CompleteRoute();
                //await CancelRoute();
                //await AddStopToRoute();
                //await UpdateStopStatus();
            }
        }

        private async Task GetRouteById()
        {
            Guid routeId = Guid.Parse("A1234567-1234-1234-1234-1234567890A1");
            try
            {
                _logger.LogInformation($"Fetching route with ID: {routeId}");
                var route = await _routeServiceClient.GetRouteAsync(routeId);

                if (route != null)
                {
                    Console.WriteLine($"Route Found: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving route with ID {routeId}");
                Console.WriteLine($"Error retrieving route: {ex.Message}");
            }
        }

        private async Task GetAllRoutes()
        {
            try
            {
                _logger.LogInformation("Fetching all routes");
                var (routes, totalCount, pageCount) = await _routeServiceClient.GetAllRoutesAsync(pageSize: 10, pageNumber: 1);

                Console.WriteLine($"Total Routes: {totalCount}");
                Console.WriteLine($"Page Count: {pageCount}");

                foreach (var route in routes)
                {
                    Console.WriteLine($"Route Found: ID={route.Id}");
                    Console.WriteLine($"Name: {route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Stops.Count}");
                    Console.WriteLine("---");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing routes");
                Console.WriteLine($"Error listing routes: {ex.Message}");
            }
        }

        private async Task GetRoutesByVehicle()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            try
            {
                _logger.LogInformation($"Fetching routes for vehicle ID: {vehicleId}");
                var routes = await _routeServiceClient.GetRoutesByVehicleAsync(vehicleId);

                if (routes.Count > 0)
                {
                    Console.WriteLine($"Found {routes.Count} routes for vehicle {vehicleId}:");
                    foreach (var route in routes)
                    {
                        Console.WriteLine($"Route ID: {route.Id}");
                        Console.WriteLine($"Name: {route.Name}");
                        Console.WriteLine($"Driver ID: {route.DriverId}");
                        Console.WriteLine($"Start Time: {DateTime.Parse(route.StartTime)}");
                        Console.WriteLine($"Stops: {route.Stops.Count}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No routes found for vehicle {vehicleId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving routes for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving routes: {ex.Message}");
            }
        }

        private async Task GetRoutesByDriver()
        {
            Guid driverId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Fetching routes for driver ID: {driverId}");
                var routes = await _routeServiceClient.GetRoutesByDriverAsync(driverId);

                if (routes.Count > 0)
                {
                    Console.WriteLine($"Found {routes.Count} routes for driver {driverId}:");
                    foreach (var route in routes)
                    {
                        Console.WriteLine($"Route ID: {route.Id}");
                        Console.WriteLine($"Name: {route.Name}");
                        Console.WriteLine($"Vehicle ID: {route.VehicleId}");
                        Console.WriteLine($"Start Time: {DateTime.Parse(route.StartTime)}");
                        Console.WriteLine($"Stops: {route.Stops.Count}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No routes found for driver {driverId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving routes for driver {driverId}");
                Console.WriteLine($"Error retrieving routes: {ex.Message}");
            }
        }

        private async Task GetRoutesByStatus()
        {
            string status = "Active";
            try
            {
                _logger.LogInformation($"Fetching routes with status: {status}");
                var routes = await _routeServiceClient.GetRoutesByStatusAsync(status);

                if (routes.Count > 0)
                {
                    Console.WriteLine($"Found {routes.Count} routes with status {status}:");
                    foreach (var route in routes)
                    {
                        Console.WriteLine($"Route ID: {route.Id}");
                        Console.WriteLine($"Name: {route.Name}");
                        Console.WriteLine($"Vehicle ID: {route.VehicleId}");
                        Console.WriteLine($"Driver ID: {route.DriverId}");
                        Console.WriteLine($"Start Time: {DateTime.Parse(route.StartTime)}");
                        Console.WriteLine($"Stops: {route.Stops.Count}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No routes found with status {status}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving routes with status {status}");
                Console.WriteLine($"Error retrieving routes: {ex.Message}");
            }
        }

        private async Task GetRoutesByDateRange()
        {
            DateTime startDate = DateTime.UtcNow.AddDays(-7);
            DateTime endDate = DateTime.UtcNow.AddDays(7);
            try
            {
                _logger.LogInformation($"Fetching routes from {startDate} to {endDate}");
                var routes = await _routeServiceClient.GetRoutesByDateRangeAsync(startDate, endDate);

                if (routes.Count > 0)
                {
                    Console.WriteLine($"Found {routes.Count} routes in date range:");
                    foreach (var route in routes)
                    {
                        Console.WriteLine($"Route ID: {route.Id}");
                        Console.WriteLine($"Name: {route.Name}");
                        Console.WriteLine($"Vehicle ID: {route.VehicleId}");
                        Console.WriteLine($"Driver ID: {route.DriverId}");
                        Console.WriteLine($"Start Time: {DateTime.Parse(route.StartTime)}");
                        Console.WriteLine($"Stops: {route.Stops.Count}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No routes found in date range {startDate} to {endDate}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving routes from {startDate} to {endDate}");
                Console.WriteLine($"Error retrieving routes: {ex.Message}");
            }
        }

        private async Task CreateRoute()
        {
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            Guid driverId = Guid.NewGuid();
            var stops = new List<RouteStop>
            {
                new RouteStop
                {
                    Address = "123 Main St, City, CA",
                    Latitude = 37.7749f,
                    Longitude = -122.4194f,
                    SequenceNumber = 1
                },
                new RouteStop
                {
                    Address = "456 Elm St, City, CA",
                    Latitude = 37.7849f,
                    Longitude = -122.4294f,
                    SequenceNumber = 2
                }
            };

            try
            {
                _logger.LogInformation("Creating a new route");
                var route = await _routeServiceClient.CreateRouteAsync(
                    name: "Delivery Route 1",
                    vehicleId: vehicleId,
                    driverId: driverId,
                    startTime: DateTime.UtcNow,
                    stops: stops
                );

                Console.WriteLine($"Route Created: ID={route.Route.Id}");
                Console.WriteLine($"Name: {route.Route.Name}");
                Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                Console.WriteLine($"Stops: {route.Route.Stops.Count}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating route");
                Console.WriteLine($"Error creating route: {ex.Message}");
            }
        }

        private async Task UpdateRoute()
        {
            Guid routeId = Guid.NewGuid();
            Guid vehicleId = Guid.Parse("8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C");
            Guid driverId = Guid.NewGuid();

            try
            {
                _logger.LogInformation($"Updating route with ID: {routeId}");
                var route = await _routeServiceClient.UpdateRouteAsync(
                    routeId: routeId,
                    name: "Updated Delivery Route",
                    vehicleId: vehicleId,
                    driverId: driverId,
                    startTime: DateTime.UtcNow.AddDays(1)
                );

                if (route != null)
                {
                    Console.WriteLine($"Route Updated: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating route with ID {routeId}");
                Console.WriteLine($"Error updating route: {ex.Message}");
            }
        }

        private async Task DeleteRoute()
        {
            Guid routeId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Deleting route with ID: {routeId}");
                var (success, message) = await _routeServiceClient.DeleteRouteAsync(routeId);

                if (success)
                {
                    Console.WriteLine($"Route deleted successfully: {message}");
                }
                else
                {
                    Console.WriteLine($"Failed to delete route: {message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting route with ID {routeId}");
                Console.WriteLine($"Error deleting route: {ex.Message}");
            }
        }

        private async Task OptimizeRoute()
        {
            Guid routeId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Optimizing route with ID: {routeId}");
                var route = await _routeServiceClient.OptimizeRouteAsync(routeId);

                if (route != null)
                {
                    Console.WriteLine($"Route Optimized: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error optimizing route with ID {routeId}");
                Console.WriteLine($"Error optimizing route: {ex.Message}");
            }
        }

        private async Task StartRoute()
        {
            Guid routeId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Starting route with ID: {routeId}");
                var route = await _routeServiceClient.StartRouteAsync(routeId);

                if (route != null)
                {
                    Console.WriteLine($"Route Started: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error starting route with ID {routeId}");
                Console.WriteLine($"Error starting route: {ex.Message}");
            }
        }

        private async Task CompleteRoute()
        {
            Guid routeId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Completing route with ID: {routeId}");
                var route = await _routeServiceClient.CompleteRouteAsync(routeId);

                if (route != null)
                {
                    Console.WriteLine($"Route Completed: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing route with ID {routeId}");
                Console.WriteLine($"Error completing route: {ex.Message}");
            }
        }

        private async Task CancelRoute()
        {
            Guid routeId = Guid.NewGuid();
            string reason = "Cancelled due to scheduling conflict";
            try
            {
                _logger.LogInformation($"Cancelling route with ID: {routeId}");
                var route = await _routeServiceClient.CancelRouteAsync(routeId, reason);

                if (route != null)
                {
                    Console.WriteLine($"Route Cancelled: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling route with ID {routeId}");
                Console.WriteLine($"Error cancelling route: {ex.Message}");
            }
        }

        private async Task AddStopToRoute()
        {
            Guid routeId = Guid.NewGuid();
            var stop = new RouteStop
            {
                Address = "789 Oak St, City, CA",
                Latitude = 37.7949f,
                Longitude = -122.4394f,
                SequenceNumber = 3
            };

            try
            {
                _logger.LogInformation($"Adding stop to route with ID: {routeId}");
                var route = await _routeServiceClient.AddStopToRouteAsync(routeId, stop);

                if (route != null)
                {
                    Console.WriteLine($"Stop Added to Route: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route with ID {routeId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding stop to route with ID {routeId}");
                Console.WriteLine($"Error adding stop to route: {ex.Message}");
            }
        }

        private async Task UpdateStopStatus()
        {
            Guid routeId = Guid.NewGuid();
            Guid stopId = Guid.NewGuid();
            try
            {
                _logger.LogInformation($"Updating stop status for stop {stopId} in route {routeId}");
                var route = await _routeServiceClient.UpdateStopStatusAsync(routeId, stopId);

                if (route != null)
                {
                    Console.WriteLine($"Stop Status Updated for Route: ID={route.Route.Id}");
                    Console.WriteLine($"Name: {route.Route.Name}");
                    Console.WriteLine($"Vehicle ID: {route.Route.VehicleId}");
                    Console.WriteLine($"Driver ID: {route.Route.DriverId}");
                    Console.WriteLine($"Start Time: {DateTime.Parse(route.Route.StartTime)}");
                    Console.WriteLine($"Stops: {route.Route.Stops.Count}");
                }
                else
                {
                    Console.WriteLine($"Route or stop not found: Route ID={routeId}, Stop ID={stopId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating stop status for stop {stopId} in route {routeId}");
                Console.WriteLine($"Error updating stop status: {ex.Message}");
            }
        }
    }
}