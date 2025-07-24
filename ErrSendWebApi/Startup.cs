using ErrSendApplication;
using ErrSendApplication.Mappings;
using ErrSendPersistensTelegram;
using ErrSendWebApi.ExceptionMidlevare;
using ErrSendWebApi.Middleware.Culture;
using ErrSendWebApi.Serviсe;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ErrSendApplication.Interfaces.Client;
using ErrSendApplication.Interfaces.Service;

namespace ErrSendWebApi
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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
            services.AddControllers();
            

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

                // c.OperationFilter<AddTimeAndTimeZoneOperationFilter>(); // Закоментовано, бо викликає помилки
            });
            services.AddSingleton<ICurrentService, CurrentService>();
            services.AddHttpContextAccessor();
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
