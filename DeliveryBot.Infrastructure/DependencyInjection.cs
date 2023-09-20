using DeliveryBot.Application.Abstractions;
using DeliveryBot.Application.Configurations;
using DeliveryBot.Application.ResponseModel;
using DeliveryBot.Application.Services;
//using DeliveryBot.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace DeliveryBot.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
      
        services.AddHttpClient("telegram_bot_client")
              .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
              {
                  BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                  TelegramBotClientOptions options = new(botConfig.BotToken);
                  return new TelegramBotClient(options, httpClient);
              });


        services.AddScoped<UpdateHandler>();
        services.AddScoped<ReceiverService>();
       // services.AddScoped<ILanguageResponseModel, LanguageService>();
        services.AddHostedService<PollingService>();

        services.Configure<BotConfiguration>(configuration.GetSection(nameof(BotConfiguration)));

     //   services.Configure<LanguageResponseModel>(configuration.GetSection(nameof(LanguageResponseModel)));





        return services;
    }
}