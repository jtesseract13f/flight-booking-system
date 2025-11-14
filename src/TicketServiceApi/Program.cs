using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketServiceApi.DAL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TicketDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
var app = builder.Build();

try //Migrator
{
    using var scope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    scope.ServiceProvider.GetRequiredService<TicketDbContext>().Database.Migrate();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/manage/health", () => StatusCodes.Status200OK);
var apiV1 = app.MapGroup("/api/v1");

apiV1.MapGet("/tickets", ([FromHeader(Name = "X-User-Name")] string apiKey) => "all user bilets");

app.Run();