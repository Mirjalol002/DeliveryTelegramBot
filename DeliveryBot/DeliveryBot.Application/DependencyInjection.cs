using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OneCode.Common.Responding;

namespace DeliveryBot.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(DependencyInjection).Assembly);

            services.AddSingleton<LocalizationService>();
            services.AddSingleton<ResponseService>();

            return services;
        }
    }
}