using LumenID.Backend.Contexts.Accounts;
using LumenID.Backend.Contexts.Clients;
using LumenID.Backend.Handlers;
using LumenID.Backend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TokenGenerator>();
builder.Services.AddSingleton(sp => {
    var connectionString = Environment.GetEnvironmentVariable("ACCOUNTS_DB_CONNECTION_STRING") ??
                           throw new InvalidOperationException(
                           "ACCOUNTS_DB_CONNECTION_STRING environment variable is not set.");
    connectionString += "Database=accounts;";

    var options = new DbContextOptionsBuilder<AccountsDbContext>()
        .UseMySQL(connectionString)
        .Options;

    return new AccountsDbContext(options);
});
builder.Services.AddSingleton(sp => {
    var connectionString = Environment.GetEnvironmentVariable("ACCOUNTS_DB_CONNECTION_STRING") ??
                           throw new InvalidOperationException(
                           "ACCOUNTS_DB_CONNECTION_STRING environment variable is not set.");
    connectionString += "Database=oauth_clients;";

    var options = new DbContextOptionsBuilder<OAuthClientsDbContext>()
        .UseMySQL(connectionString)
        .Options;

    return new OAuthClientsDbContext(options);
});

builder.Services.AddGrpc();

builder.Services.AddAuthentication()
    .AddScheme<AuthenticationSchemeOptions, UsersAuthenticationHandler>("UserAuthentication", options => {});
builder.Services.AddAuthorization(options => {
    options.AddPolicy("users", policy => {
        policy.RequireAuthenticatedUser()
            .AddAuthenticationSchemes("UserAuthentication");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();
app.MapGrpcService<RegisterServiceImpl>();
app.MapGrpcService<AuthenticateServiceImpl>();
app.MapGrpcService<GrantServiceImpl>();
app.MapGet("/",
() =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
