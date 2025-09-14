using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using LumenID.Protos.V0.Services;
using LumenID.Protos.V0.Types;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;

namespace LumenID.Frontend.Components.Pages;

public partial class Grant : ComponentBase {

    private enum HandleGrantStatus {
        NONE,
        READY,
        SENDING,
        REDIRECTING,
        ERROR
    }

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    
    private Grpc.Core.Metadata? _metadata;
    private GrantService.GrantServiceClient _grantServiceClient = null!;
    private int _firstResponseSpeed = 0;
    private bool _isInitialized = false;
    private HandleGrantStatus _grantStatus = HandleGrantStatus.NONE;
    
    private string _clientId = string.Empty;
    private string _redirectUri = string.Empty;
    
    protected override async Task OnInitializedAsync()
    {
        if (_isInitialized) return;
        _isInitialized = true;
        
        Console.WriteLine("grant page loaded");
        
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var token = await JsRuntime.InvokeAsync<string>("window.localStorage.getItem", "token");
            var userId = await JsRuntime.InvokeAsync<string>("window.localStorage.getItem", "user-id");
            var sessionId = await JsRuntime.InvokeAsync<string>("window.localStorage.getItem", "session-id");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(sessionId))
            {
                NavigationManager.NavigateTo("login");
            } else
            {
                Console.WriteLine("{0} {1} {2}", sessionId, userId, token);
                _metadata = new Metadata()
                {
                    { "token", token },
                    { "user-id", userId },
                    { "session-id", sessionId }
                };
            }

            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queries = QueryHelpers.ParseQuery(uri.Query);

            _clientId = queries.TryGetValue("client-id", out var c) ? c.ToString() : string.Empty;
            _redirectUri = queries.TryGetValue("redirect", out var u) ? u.ToString() : string.Empty;

            if (string.IsNullOrEmpty(_clientId) || string.IsNullOrEmpty(_redirectUri))
            {
                _grantStatus = HandleGrantStatus.NONE;
                StateHasChanged();
                return;
            }
            
            var channel = GrpcChannel.ForAddress("http://localhost:5237");
            _grantServiceClient = new GrantService.GrantServiceClient(channel);

            var now = DateTime.Now;
            var re = _grantServiceClient.Ping(new Empty());
            if (re is not null)
            {
                _firstResponseSpeed = (DateTime.Now - now).Milliseconds;
                _grantStatus = HandleGrantStatus.READY;
            } else
            {
                _grantStatus = HandleGrantStatus.NONE;
            }

            StateHasChanged();
        }
        
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnGrantButtonClick()
    {
        if (_grantStatus is HandleGrantStatus.NONE)
        {
            Console.WriteLine("grant button clicked");
            return;
        }
        
        _grantStatus = HandleGrantStatus.SENDING;
        
        try
        {
            var data = new GrantRequest()
            {
                ClientId = _clientId,
                RedirectUrl = _redirectUri
            };

            var response = await _grantServiceClient.GrantAppAsync(data, _metadata);

            _redirectUri += $"?code={response.Code}";
            Console.WriteLine($"redirect: {_redirectUri}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _grantStatus = HandleGrantStatus.ERROR;
            return;
        }
        
        _grantStatus = HandleGrantStatus.REDIRECTING;
        
        await Task.CompletedTask;
    }
}

