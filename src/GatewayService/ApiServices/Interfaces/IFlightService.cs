using GatewayService.DTO;

namespace GatewayService.ApiServices.Interfaces;

public interface IFlightService
{
    Task<FlightInfo>  GetFlightInfo(string flightNumber);
    Task<List<FlightInfo>> GetAllFlightInfos();
}