using GatewayService.ApiServices;
using GatewayService.BLL;
using GatewayService.DTO;
using GatewayService.DTO.BonusServiceDtos;
using GatewayService.DTO.FlightApiDtos;
using GatewayService.DTO.TicketServiceDtos;
using Microsoft.AspNetCore.Http;
using Moq;

namespace GatewayService.Test;

public class BookingServiceTest
{
    [Fact]
    public void Test1()
    {
        
    }
    
    [Fact]
    public async Task BuyTicketTest()
    {
        var mockTicketService = new Mock<ITicketApi>();
        var mockBonusService = new Mock<IBonusApi>();
        var mockFlightService = new Mock<IFlightApi>();

        var expectedFlight = new Flight("PEN512", DateTime.UtcNow, "S-23", "Mars", 1100);
        mockFlightService
            .Setup(service => service.GetFlightInfo(It.IsAny<string>()))
            .Returns(async () => expectedFlight);
        var expectedPurchaseInfo = new PurchaseInfo(0, 1100, 1100);
        mockBonusService
            .Setup(service => service.GetPurchaseInfo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(async () => expectedPurchaseInfo);
        var expectedCreatedTicket = new CreatedTicket(Guid.NewGuid(), "LSTR", "PEN512", 1100, "PAID");
        mockTicketService
            .Setup(service => service.CreateTicket(It.IsAny<Flight>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(async () => expectedCreatedTicket);
        var expectedPrivilege = new Privilege(11, "BRONZE");
        mockBonusService
            .Setup(service => service.ChangeBalance(It.IsAny<string>(), It.IsAny<TicketPurchase>()))
            .Returns(async () => expectedPrivilege);
        
        var flightService = mockFlightService.Object;
        var ticketService = mockTicketService.Object;
        var bonusService = mockBonusService.Object;

        var bullingService = new BookingService(bonusService, flightService, ticketService);

        var result = await bullingService.BuyTicket("LSTR", new BuyTicket("PEN512", 1100, false));
        
        Assert.True(result.FlightNumber == "PEN512");
    }
    
    [Fact]
    public async Task BuyTicketThrowsNotFoundTest()
    {
        var mockTicketService = new Mock<ITicketApi>();
        var mockBonusService = new Mock<IBonusApi>();
        var mockFlightService = new Mock<IFlightApi>();

        var expectedFlight = new Flight("PEN512", DateTime.UtcNow, "S-23", "Mars", 1100);
        mockFlightService
            .Setup(service => service.GetFlightInfo(It.IsAny<string>()))
            .Returns(async () => null);
        
        var flightService = mockFlightService.Object;
        var ticketService = mockTicketService.Object;
        var bonusService = mockBonusService.Object;

        var bullingService = new BookingService(bonusService, flightService, ticketService);
        
        await Assert.ThrowsAsync<NotFoundException>(() => bullingService.BuyTicket("LSTR", new BuyTicket("PEN512", 1100, false)));
    }
    
    [Fact]
    public async Task BuyTicketThrowsBadRequestTest()
    {
        var mockTicketService = new Mock<ITicketApi>();
        var mockBonusService = new Mock<IBonusApi>();
        var mockFlightService = new Mock<IFlightApi>();

        var expectedFlight = new Flight("PEN512", DateTime.UtcNow, "S-23", "Mars", 1100);
        mockFlightService
            .Setup(service => service.GetFlightInfo(It.IsAny<string>()))
            .Returns(async () => expectedFlight);
        var expectedPurchaseInfo = new PurchaseInfo(0, 1100, 1100);
        mockBonusService
            .Setup(service => service.GetPurchaseInfo(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>()))
            .Returns(async () => expectedPurchaseInfo);
        var expectedCreatedTicket = new CreatedTicket(Guid.NewGuid(), "LSTR", "PEN512", 1100, "PAID");
        mockTicketService
            .Setup(service => service.CreateTicket(It.IsAny<Flight>(), It.IsAny<int>(), It.IsAny<string>()))
            .Returns(async () => expectedCreatedTicket);
        var expectedPrivilege = new Privilege(11, "BRONZE");
        mockBonusService
            .Setup(service => service.ChangeBalance(It.IsAny<string>(), It.IsAny<TicketPurchase>()))
            .Returns(async () => expectedPrivilege);
        
        var flightService = mockFlightService.Object;
        var ticketService = mockTicketService.Object;
        var bonusService = mockBonusService.Object;

        var bullingService = new BookingService(bonusService, flightService, ticketService);
        var fakePrice = 300;
        await Assert.ThrowsAsync<BadHttpRequestException>(() => bullingService.BuyTicket("LSTR", new BuyTicket("PEN512", 300, false)));
    }
    
    
}
