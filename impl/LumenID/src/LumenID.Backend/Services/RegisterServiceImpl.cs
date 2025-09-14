using Grpc.Core;
using LumenID.Backend.Contexts.Accounts;
using LumenID.Protos.V0.Enums;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Mvc;

namespace LumenID.Backend.Services;

public class RegisterServiceImpl(
    [FromServices] AccountsDbContext database,
    [FromServices] ILogger<RegisterServiceImpl> logger
) : RegisterService.RegisterServiceBase {
    public override async Task Register(AuthAccountModel request, IServerStreamWriter<RegisterResponse> responseStream, ServerCallContext context)
    {
        // First response
        await responseStream.WriteAsync(new RegisterResponse
        {
            Status = RegisterStatus.V0Requested,
            Message = "Account registration started."
        });
        logger.LogInformation("Register request received for email: {Email}", request.Email);

        // Email conflict check
        if (await database.IsEmailExistsAsync(request.Email) is true)
        {
            // Email conflict found
            logger.LogWarning("Email conflict found for email: {Email}", request.Email);
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Email already exists."));
        }

        // Account creation started
        await responseStream.WriteAsync(new RegisterResponse
        {
            Status = RegisterStatus.V0InAccountCreationStarted,
            Message = "Account creation started."
        });

        // Create new account
        await database.CreateNewAccountAsync(request.Email, request.Password);

        // Account created
        await responseStream.WriteAsync(new RegisterResponse
        {
            Status = RegisterStatus.V0InCreatedAccount,
            Message = "Account created."
        });

        // Completed
        await responseStream.WriteAsync(new RegisterResponse
        {
            Status = RegisterStatus.V0Completed,
            Message = "Account registration completed."
        });

        logger.LogInformation("Account registration completed for email: {Email}", request.Email);
    }
}
