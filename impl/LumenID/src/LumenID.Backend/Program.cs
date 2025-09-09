using LumenID.Backend.Contexts;
using LumenID.Backend.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddSingleton(sp => {
    var connectionString = Environment.GetEnvironmentVariable("ACCOUNTS_DB_CONNECTION_STRING") ??
                           throw new InvalidOperationException(
                           "ACCOUNTS_DB_CONNECTION_STRING environment variable is not set.");

    var options = new DbContextOptionsBuilder<AccountsDbContext>()
        .UseMySQL(connectionString)
        .Options;

    return new AccountsDbContext(options);
});

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<RegisterServiceImpl>();
app.MapGrpcService<AuthenticateServiceImpl>();
app.MapGet("/",
() =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
