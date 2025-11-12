using GatewayService.ApiServices.Interfaces;
using GatewayService.DTO;

namespace GatewayService.BLL;

public class BookingService(IBonusService bonusService, IFlightService flightService, ITicketService ticketService)
{
    /*
     * Пользователь вызывает метод GET {{baseUrl}}/api/v1/flights выбирает нужный рейс и в запросе на покупку передает:
       
       flightNumber (номер рейса) – берется из запроса /flights;
       price (цена) – берется из запроса /flights;
       paidFromBalance (оплата бонусами) – флаг, указывающий, что для оплаты билета нужно использовать бонусный счет.
       Система проверяет, что рейс с таким номером существует. Считаем что на рейсе бесконечное количество мест.
       
       Если при покупке указан флаг "paidFromBalance": true, то с бонусного счёта списываются максимальное количество баллов в отношении 1 балл – 1 рубль.
       
       Т.е. если на бонусном счете было 500 бонусов, билет стоит 1500 рублей и при покупке был указан флаг "paidFromBalance": true", то со счёта спишется 500 бонусов (в ответе будет указано "paidByBonuses": 500), а стоимость билета будет 1000 рублей (в ответе будет указано "paidByMoney": 1000). В сервисе Bonus Service в таблицу privilegeHistory будет добавлена запись о списании со счёта 500 бонусов.
       
       Если при покупке был указан флаг "paidFromBalance": false, то в ответе будет "paidByBonuses": 0, а на бонусный счет будет начислено бонусов в размере 10% от стоимости заказа. Так же в таблицу privilegeHistory будет добавлена запись о зачислении бонусов.
     */
    private readonly IBonusService _bonusService = bonusService;
    private readonly IFlightService _flightService = flightService;
    private readonly ITicketService _ticketService = ticketService;

    public async Task<PurchasedTicketInfo> BuyTicket(string username, BuyTicket request)
    {
        var flight = await _flightService.GetFlightInfo(request.FlightNumber);
        if (flight == null) throw new NotFoundException($"Flight {request.FlightNumber} not found");
        if (flight.Price != request.Price) throw new BadHttpRequestException("Incorrect price");
        
        
        await _bonusService.ChangeBalance(username, request.Price, request.PaidFromBalance);
        
        // add ticket to ticket service
        
        var purchasedTicketInfo = new PurchasedTicketInfo(
            );

        return purchasedTicketInfo;
    }
    /*
     * Возврат билета
       Билет помечается статусом CANCELED, в Bonus Service в зависимости от типа операции выполняется возврат бонусов на счёт или списание ранее начисленных. При списании бонусный счет не может стать меньше 0.
       
       DELETE {{baseUrl}}/api/v1/tickets/{{ticketUid}}
       X-User-Name: {{username}}
     */
    
    public async Task
    
    
}