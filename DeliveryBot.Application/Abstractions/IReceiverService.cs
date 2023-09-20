namespace DeliveryBot.Application.Abstractions;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}