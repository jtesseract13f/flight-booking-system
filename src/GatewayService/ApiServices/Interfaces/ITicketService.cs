using GatewayService.DTO;
using GatewayService.DTO.TicketServiceDtos;

namespace GatewayService.ApiServices.Interfaces;

public interface ITicketService
{
    public Task<CreatedTicket> CreateTicket(FlightInfo flightInfo, int price, string username);
}