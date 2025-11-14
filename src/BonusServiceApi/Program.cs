using BonusServiceApi.DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BonusDbContext>(x => x.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddSwaggerGen();

var app = builder.Build();

try //Migrator
{
    using var scope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope();
    scope.ServiceProvider.GetRequiredService<BonusDbContext>().Database.Migrate();
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

app.Run();
