using Grpc.Core;
using Grpc.Net.Client;

using LumenID.Protos.V0.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace LumenID.Frontend.Components.Pages;

public struct LogoutState {
  public LogoutState() { }

  public enum Status {
    LoggingOut,
    RemovingSession,
    LoggedOut
  }

  public const string LoggingOut = "Logging out...";
  public string RemovingSession = "Removing session...";
  public string LoggedOut = "You have been logged out.";
  public Status CurrentStatus = Status.LoggingOut;

  public void SetStatus(Status status) {
    CurrentStatus = status;
  }

  public readonly string GetStatusMessage() {
    return CurrentStatus switch {
      Status.LoggingOut => LoggingOut,
      Status.RemovingSession => RemovingSession,
      Status.LoggedOut => LoggedOut,
      _ => "Unknown status"
    };
  }
}

public partial class Logout : ComponentBase {
  [Inject]
  private ILogger<Logout> Logger { get; set; } = null!;
  [Inject]
  private IJSRuntime JSRuntime { get; set; } = null!;
  [Inject]
  private NavigationManager NavigationManager { get; set; } = null!;

  private LogoutState _state = new();
  private bool _showRefreshButton = false;

  private string _sessiondId = string.Empty;
  private string _token = string.Empty;
  private string _expiresAt = string.Empty;
  private string _accountId = string.Empty;

  private AuthenticateService.AuthenticateServiceClient _authClient = null!;

  protected override async Task OnInitializedAsync() {
    var channel = GrpcChannel.ForAddress("http://localhost:5237");
    _authClient = new AuthenticateService.AuthenticateServiceClient(channel);
    await base.OnInitializedAsync();
  }

  protected override async Task OnAfterRenderAsync(bool firstRender) {
    if (firstRender) {
      _sessiondId = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", "session-id") ?? string.Empty;
      _token = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", "token") ?? string.Empty;
      _accountId = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", "user-id") ?? string.Empty;
      _expiresAt = await JSRuntime.InvokeAsync<string>("window.localStorage.getItem", "expires-at") ?? string.Empty;

      _state.SetStatus(LogoutState.Status.LoggingOut);
      StateHasChanged();

      // logout process
      try {
        var metadata = new Metadata {
          { "session-id", _sessiondId },
          { "token", _token },
          { "user-id", _accountId },
          { "expires-at", _expiresAt }
        };

        await _authClient.UnAuthenticateAsync(new() {
          SessionId = _sessiondId
        }, metadata);
      } catch (Exception ex) {
        Logger.LogError(ex, "Error during logout");
        return;
      }

      _state.SetStatus(LogoutState.Status.RemovingSession);
      StateHasChanged();

      // Clear local storage
      try {
        await JSRuntime.InvokeVoidAsync("window.localStorage.removeItem", "session-id");
        await JSRuntime.InvokeVoidAsync("window.localStorage.removeItem", "token");
        await JSRuntime.InvokeVoidAsync("window.localStorage.removeItem", "user-id");
        await JSRuntime.InvokeVoidAsync("window.localStorage.removeItem", "expires-at");
      } catch (Exception ex) {
        Logger.LogError(ex, "Error clearing local storage");
        return;
      }

      _state.SetStatus(LogoutState.Status.LoggedOut);
      StateHasChanged();

      NavigationManager.NavigateTo("/");
    }

    await base.OnAfterRenderAsync(firstRender);
  }
}
