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
                            });

            // Валідатори вже зареєстровані вище, повторна реєстрація не потрібна

            // Перевизначаємо фабрику для Typed Client, щоб передати валідатор у конструктор
            services.AddTransient<IHttpClientWr>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(typeof(StandartHttpClient).FullName!);
                var validator = sp.GetRequiredService<IValidator<(string url, HttpContent content, string? token)>>();
                return new StandartHttpClient(httpClient, validator);
            });

            services.AddTransient<ITelegramService, TelegramService>();

            return services;
        }
    }
}
