using ErrSendApplication;
using ErrSendPersistensTelegram;
using ErrSendWebApi.ExceptionMidlevare;
using ErrSendWebApi.Middleware.Culture;
using ErrSendWebApi.Serviсe;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ErrSendApplication.Interfaces.Client;
using ErrSendApplication.Interfaces.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ErrSendApplication.Mappings;
using ErrSendWebApi.TimeZone;
using FluentValidation;
using ErrSendWebApi.Validators;
using FluentValidation.AspNetCore;

namespace ErrSendWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            var basePath = Directory.GetCurrentDirectory();
            Configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(config =>
            {
                config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
                config.AddProfile(new AssemblyMappingProfile(typeof(IHttpClientWr).Assembly));
            });

            services.AddApplication(Configuration);
            services.AddPersistenceTelegram(Configuration);
            
            services
                .AddControllers()
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ErrorSender.Api",
                    Version = "v1",
                    Description = "Error Sender API"
                });
                
                // Реєструємо OperationFilter через DI (конструкторні залежності будуть резолвитися автоматично)
                c.OperationFilter<AddTimeAndTimeZoneOperationFilter>();
                
                // Додаємо підтримку JWT Bearer авторизації
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            // Зареєструємо залежності, які потрібні для OperationFilter
            services.AddTransient<IValidator<(OpenApiOperation operation, string responseKey, string contentType)>, OperationFilterValidator>();
            services.AddTransient<AddTimeAndTimeZoneOperationFilter>();
            services.AddSingleton<ICurrentService, CurrentService>();
            services.AddHttpContextAccessor();
            var jwtConfig = services.BuildServiceProvider().GetRequiredService<ErrSendApplication.Common.Configs.JwtConfig>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)) // HMAC-SHA1
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        // Більше не надсилаємо повідомлення у Telegram при 401
                        await System.Threading.Tasks.Task.CompletedTask;
                    }
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = string.Empty;
                config.SwaggerEndpoint("swagger/v1/swagger.json", "ErrorSender.Api");
                config.InjectStylesheet("/swagger/Ui/theme-feeling-blue.css");
            });

            app.UseCustomExceptionHandler();
            app.UseCulture();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
