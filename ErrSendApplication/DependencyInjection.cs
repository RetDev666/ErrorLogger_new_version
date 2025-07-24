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
            if (!string.IsNullOrEmpty(envSecret))
            {
                jwtConfig.Secret = envSecret;
            }
            if (string.IsNullOrWhiteSpace(jwtConfig.Secret) || jwtConfig.Secret.Length < 32)
            {
                var bytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(bytes);
                }
                jwtConfig.Secret = Convert.ToBase64String(bytes);
            }
            services.AddSingleton(jwtConfig);
            services.AddSingleton<IJwtTokenService>(provider =>
                new JwtTokenService(jwtConfig.Secret, jwtConfig.ExpiryMinutes));

            return services;
        }
    }
}
