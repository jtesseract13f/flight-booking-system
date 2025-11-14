using System.ComponentModel.DataAnnotations;
using GatewayService.BLL;
using GatewayService.DTO;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = error switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new
        {
            error = error?.Message,
            code = context.Response.StatusCode
        };

        await context.Response.WriteAsJsonAsync(problem);
    });
});

app.MapGet("/manage/health", () => StatusCodes.Status200OK);

var apiV1 = app.MapGroup("/api/v1");

//!!!
apiV1.MapGet("/flights", ([FromQuery] int page, [FromQuery] int size) => 
    "")
    .WithDescription("Получить список рейсов")
    .WithOpenApi();

//!!!
apiV1.MapGet("/privilege", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Получить информацию о состоянии бонусного счета")
    .WithOpenApi();

apiV1.MapPost("/tickets", async ([FromHeader(Name = "X-User-Name")]string username, BuyTicket ticket, BookingService service) => 
    await service.BuyTicket(username, ticket))
    .WithDescription("Покупка билета")
    .WithOpenApi();

//!!!
apiV1.MapGet("/tickets/{ticketUid}", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Информация по конкретному билету")
    .WithOpenApi();

//!!!
apiV1.MapGet("/tickets", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Информация по всем билетам пользователя")
    .WithOpenApi();

//!!!
apiV1.MapGet("/me", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Информация о пользователе")
    .WithOpenApi();

//!!!
apiV1.MapGet("/privilege", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Получить информацию о состоянии бонусного счета после покупки билета")
    .WithOpenApi();

apiV1.MapDelete("/tickets/{ticketUid}", ([FromHeader(Name = "X-User-Name")]string username) => "")
    .WithDescription("Возврат билета")
    .WithOpenApi();

app.Run();