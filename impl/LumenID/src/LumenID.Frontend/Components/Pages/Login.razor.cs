using System.Security.Cryptography;
using System.Text;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Components;

namespace LumenID.Frontend.Components.Pages;

public partial class Login : ComponentBase {
    [Inject] private ILogger<Login> Logger { get; set; } = null!;

    private string _email = string.Empty;
    private string _password = string.Empty;
    private bool _isEmailInvalid;
    private bool _isPasswordInvalid;
    private string _statusMessage = string.Empty;
    private AuthenticateService.AuthenticateServiceClient _authenticateServiceClient = null!;

    protected override async Task OnInitializedAsync()
    {
        var channel = Grpc.Net.Client.GrpcChannel.ForAddress("http://localhost:5237");
        _authenticateServiceClient = new AuthenticateService.AuthenticateServiceClient(channel);
        await base.OnInitializedAsync();
    }

    private async Task HandleSubmit()
    {
        // Check email is valid
        if (string.IsNullOrWhiteSpace(_email) || !_email.Contains("@") || !_email.Split('@')[1].Contains('.'))
        {
            _isEmailInvalid = true;
            return;
        }

        // Check password is valid
        if (string.IsNullOrWhiteSpace(_password) || _password.Length < 8)
        {
            _isPasswordInvalid = true;
            return;
        }

        _isEmailInvalid = false;
        _isPasswordInvalid = false;

        // Hash password
        var passwordBytes = Encoding.UTF8.GetBytes(_password);
        var hashedBytes = SHA256.HashData(passwordBytes);
        var hashedPassword = Convert.ToBase64String(hashedBytes);

        var request = new AuthAccountModel
        {
            Email = _email,
            Password = hashedPassword
        };

        try
        {
            var response = await _authenticateServiceClient.AuthenticateAsync(request);
            Logger.LogInformation("Login success for {uid} with {token} by {sid}", response.UserId, response.Token, response.SessionId);
        }
        catch (Exception e)
        {
            _statusMessage = "Something went wrong... Please try again later.";
            Logger.LogError("{E}", e.Message);
        }

        // if success, store token in local storage and redirect to <redirect>
        await Task.CompletedTask;
    }
}
