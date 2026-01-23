using GatewayService.ApiServices;
using GatewayService.DTO;
using GatewayService.DTO.BonusServiceDtos;

namespace GatewayService.BLL;

public class BookingService(IBonusApi bonusService, IFlightApi flightService, ITicketApi ticketService)
{
    public async Task<PurchasedTicketInfo> BuyTicket(string username, BuyTicket request)
    {
        var flight = await flightService.GetFlightInfo(request.FlightNumber);
        if (flight == null) throw new NotFoundException($"Flight {request.FlightNumber} not found");
        if (flight.Price != request.Price) throw new BadHttpRequestException("Incorrect price");

        var purchaseInfo = await bonusService.GetPurchaseInfo(username, request.Price, request.PaidFromBalance);
        var createdTicket = await ticketService.CreateTicket(flight, purchaseInfo.Price, username);
        var privilege = await bonusService.ChangeBalance(username, new TicketPurchase(createdTicket, purchaseInfo.Price, request.PaidFromBalance));
        
        var purchasedTicketInfo = new PurchasedTicketInfo(
            createdTicket.TicketUid,
            createdTicket.FlightNumber,
            flight.FromAirport,
            flight.ToAirport,
            flight.Date,
            purchaseInfo.Price,
            purchaseInfo.PaidByMoney,
            purchaseInfo.PaidByBonuses,
            createdTicket.Status,
            privilege
            );

        return purchasedTicketInfo;
    }

    public async Task CancelTicket(string username, Guid ticketUid)
    {
        var result = await bonusService.RevertPurchase(username, ticketUid); //TO DO: add endpoint to bonus service
        var result2 = await ticketService.CancelTicket(ticketUid);
        //TODO: revert transaction
        
        if (result != ticketUid || result2 != ticketUid) throw new Exception($"Can't cancel purchase for ticket {ticketUid}");
    }

    public async Task<UserInfo?> GetUser(string username)
    {
        var privilege = await bonusService.GetBalanceInfo(username);
        if (privilege == null) throw new NotFoundException($"User {username} not found");
        var tickets =  await GetUserTickets(username);
        return new UserInfo(tickets,  new Privilege(privilege.Balance, privilege.Status));
    }

    public async Task<TicketInfo> GetTicketInfo(string username, Guid ticketUid)
    {
        var ticket = await ticketService.GetTicket(ticketUid, username); 
        if (ticket == null) throw new NotFoundException($"Ticket {ticketUid} not found");
        var flight = await flightService.GetFlightInfo(ticket.FlightNumber);
        if (flight == null) throw new NotFoundException($"Flight {ticket.FlightNumber} not found");

        return new TicketInfo(
            ticket.TicketUid,
            ticket.FlightNumber,
            flight.FromAirport,
            flight.ToAirport,
            flight.Date,
            ticket.Price,
            ticket.Status);
    }
    
    public async Task<IEnumerable<TicketInfo>> GetUserTickets(string username)
    {
        var ticketsRaw = await ticketService.GetUserTickets(username);
        var flights = (await flightService.GetAllFlightInfos(1, 100)).ToDictionary(x => x.FlightNumber, x => x);
        return ticketsRaw.Select(x => new TicketInfo(
            x.TicketUid,
            x.FlightNumber,
            flights[x.FlightNumber].FromAirport,
            flights[x.FlightNumber].ToAirport,
            flights[x.FlightNumber].Date,
            flights[x.FlightNumber].Price,
            x.Status));
    }
    
    
}