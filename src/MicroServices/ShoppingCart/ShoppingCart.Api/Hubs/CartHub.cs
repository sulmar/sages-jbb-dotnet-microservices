using Microsoft.AspNetCore.SignalR;

namespace ShoppingCart.Api.Hubs;

public class CartHub(ILogger<CartHub> _logger) : Hub
{
    public override Task OnConnectedAsync()
    {
        // zła praktyka
        // _logger.LogInformation($"Connected {Context.ConnectionId}");

        // dobra praktyka
        _logger.LogInformation("Connected {ConnectionId}", Context.ConnectionId);

        // this.Context.User.Claims.
        string sessionId = "user:1";

        this.Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Disconnected {ConnectionId}", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}
