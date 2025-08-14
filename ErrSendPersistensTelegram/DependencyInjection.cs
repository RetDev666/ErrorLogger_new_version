using ErrSendApplication.Common.Configs;
using ErrSendApplication.Interfaces.Client;
using ErrSendApplication.Interfaces.Telegram;
using ErrSendPersistensTelegram.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System.Reflection;

namespace ErrSendPersistensTelegram
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceTelegram(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TelegramConfig>(configuration.GetSection("Telegram"));

            // Реєструємо FluentValidation валідатори
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddHttpClient<IHttpClientWr, StandartHttpClient>()
                            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                             {
                                 ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
                             })
                            .ConfigureHttpClient(client =>
                            {
                                var serverUrl = configuration.GetValue<string>("serverUrl") ?? "http://localhost:5001/";
                                client.BaseAddress = new Uri(serverUrl);
                            })
                            .AddTypedClient((services, provider) =>
                            {
                                var validator = provider.GetRequiredService<IValidator<(string url, HttpContent content, string? token)>>();
                                return new StandartHttpClient(provider.GetRequiredService<HttpClient>(), validator);
                            });

            services.AddTransient<ITelegramService, TelegramService>();

            return services;
        }
    }
}
