using ErrSendApplication.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ErrSendApplication.Authorization;
using ErrSendApplication.Common.Configs;
using ErrSendApplication.Interfaces.Authorization;
using System.Security.Cryptography;

namespace ErrSendApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblies(new[] { Assembly.GetExecutingAssembly() });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

            var jwtConfig = new JwtConfig();
            configuration.GetSection("JwtConfig").Bind(jwtConfig);
            var envSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            
            // Валідація через FluentValidation замість ручних перевірок
            var envValidator = services.BuildServiceProvider().GetRequiredService<IValidator<(string? envSecret, string configSecret, int configLength)>>();
            
            // Валідація envSecret через FluentValidation
            if (envSecret != null)
            {
                var envValidationResult = envValidator.Validate((envSecret, envSecret, envSecret.Length));
                if (envValidationResult.IsValid)
                {
                    jwtConfig.Secret = envSecret;
                }
            }
            
            var validationResult = envValidator.Validate((envSecret, jwtConfig.Secret, jwtConfig.Secret?.Length ?? 0));
            if (!validationResult.IsValid)
            {
                // Якщо конфігурація невалідна, генеруємо новий секрет
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                jwtConfig.Secret = Convert.ToBase64String(bytes);
            }
            
            services.AddSingleton(jwtConfig);
            services.AddSingleton<IJwtTokenService>(provider =>
                new JwtTokenService(jwtConfig.Secret, jwtConfig.ExpiryMinutes, 
                    provider.GetRequiredService<IValidator<JwtConfig>>()));

            return services;
        }
    }
}
