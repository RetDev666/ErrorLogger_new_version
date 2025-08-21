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
            var envValidator = new Validators.EnvironmentConfigValidator();
            
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
            
            // Попередньо валідовуємо JwtConfig власним інстансом валідатора, щоб не тягнути scoped залежності у singleton
            var jwtConfigValidator = new Validators.JwtConfigValidator();
            var jwtConfigValidationResult = jwtConfigValidator.Validate(jwtConfig);
            if (!jwtConfigValidationResult.IsValid)
            {
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                jwtConfig.Secret = Convert.ToBase64String(bytes);
            }

            services.AddSingleton(jwtConfig);
            services.AddSingleton<IJwtTokenService>(_ =>
                new JwtTokenService(jwtConfig.Secret, jwtConfig.ExpiryMinutes, jwtConfigValidator));

            return services;
        }
    }
}
