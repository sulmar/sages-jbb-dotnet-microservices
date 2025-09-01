
using Microsoft.AspNetCore.Connections;
using StackExchange.Redis;
using System.Diagnostics.SymbolStore;

namespace ShoppingCart.Api.BackgroundServices;

public class MyWorker(ILogger<MyWorker> _logger) : BackgroundService
{    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("DoWork");

            await Task.Delay(1000);
        }
    }
}


public class NotificationWorker(ILogger<NotificationWorker> _logger, IConnectionMultiplexer connection) : BackgroundService
{
    const string PubSubChannel = "events:product-updated";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var sub = connection.GetSubscriber();

        await sub.SubscribeAsync(PubSubChannel, (channel, message) =>
        {
            _logger.LogInformation("Received on {Channel}: {Message}", channel, message);
        });

        _logger.LogInformation("Subscibed to {Channel}", PubSubChannel);
    }
}