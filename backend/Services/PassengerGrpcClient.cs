using Grpc.Net.Client;
using PassengerGrpcService;
using Microsoft.Extensions.Configuration;
using FlightApp.Models;  // AdminPassengerView sınıfı için

namespace FlightApp.Services
{
    public class PassengerGrpcClient
{
    private readonly PassengerService.PassengerServiceClient _client;
    private readonly ILogger<PassengerGrpcClient> _logger;

    public PassengerGrpcClient(IConfiguration configuration, ILogger<PassengerGrpcClient> logger)
    {
        _logger = logger;
        var address = configuration["GrpcSettings:PassengerServiceUrl"];
        _logger.LogInformation($"Connecting to gRPC server at: {address}");

        try 
        {
            var channel = GrpcChannel.ForAddress(address);
            _client = new PassengerService.PassengerServiceClient(channel);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating gRPC client: {ex.Message}");
            throw;
        }
    }

    public async Task<List<AdminPassengerView>> GetPassengersAsync()
    {
        try
        {
            _logger.LogInformation("Calling GetPassengers gRPC method");
            var request = new GetPassengersRequest();
            var response = await _client.GetPassengersAsync(request);
            _logger.LogInformation($"Got {response.Passengers.Count} passengers from gRPC server");

            return response.Passengers.Select(p => new AdminPassengerView
            {
                PassengerId = p.PassengerId,
                Gender = p.Gender,
                MaskedTcNo = p.MaskedTcNo,
                PassengerName = p.PassengerName,
                PassengerSurname = p.PassengerSurname,
                DateOfBirth = DateTime.Parse(p.DateOfBirth)
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling GetPassengers: {ex.Message}");
            throw;
        }
    }
}
}