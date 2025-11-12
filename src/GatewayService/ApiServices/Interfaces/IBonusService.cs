using GatewayService.DTO;

namespace GatewayService.ApiServices.Interfaces;

public interface IBonusService
{
    public Task<BalanceInfo> GetBalanceInfo(string username);
    public Task ChangeBalance(string username, int ticketPrice, bool useBalance = true);
}