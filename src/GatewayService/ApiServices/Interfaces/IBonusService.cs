using GatewayService.DTO;
using GatewayService.DTO.BonusServiceDtos;
using GatewayService.DTO.TicketServiceDtos;

namespace GatewayService.ApiServices.Interfaces;

public interface IBonusService
{
    public Task<BalanceInfo> GetBalanceInfo(string username);
    public Task<Privilege> ChangeBalance(string username, CreatedTicket ticket, int price, bool useBalance);
    public Task<PurchaseInfo> GetPurchaseInfo(string username, int price, bool useBalance);
    public Task<Guid> RevertPurchase(string username, Guid ticketGuid);    //returns cancelled ticket uid
}