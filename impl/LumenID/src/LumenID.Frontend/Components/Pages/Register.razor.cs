using System.Security.Cryptography;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Components;

namespace LumenID.Frontend.Components.Pages;

public partial class Register : ComponentBase
{
    [Inject]
    private ILogger<Register> _logger { get; set; } = null!;
    [Inject]
    private NavigationManager _navigationManager { get; set; } = null!;

    private string _email = string.Empty;
    private string _password = string.Empty;

    private bool _isEmailInvalid = false;
    private bool _isPasswordInvalid = false;

    private RegisterService.RegisterServiceClient _apiClient = null!;
    private string _statusMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var channel = GrpcChannel.ForAddress("http://localhost:5237");
        _apiClient = new RegisterService.RegisterServiceClient(channel);

        await base.OnInitializedAsync();
    }

    public async Task HandleSubmit()
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

        // Call API
        var request = new AuthAccountModel
        {
            Email = _email,
            Password = hashedPassword,
        };

        try
        {
            var responses = _apiClient.Register(request);

            while (await responses.ResponseStream.MoveNext(CancellationToken.None))
            {
                var response = responses.ResponseStream.Current;
                _logger.LogInformation("Register status: {Status} - {Message}", response.Status, response.Message);
                _statusMessage = response.Message;
            }

            _navigationManager.NavigateTo("/login");
        }
        catch (RpcException rpcEx)
        {
            _logger.LogWarning("gRPC error occurred while registering with {Message}", rpcEx.Status.Detail);
            _statusMessage = rpcEx.Status.Detail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while registering.");
            _statusMessage = "An error occurred while registering. Please try again 'later'.";
        }

        await Task.CompletedTask;
    }
}
