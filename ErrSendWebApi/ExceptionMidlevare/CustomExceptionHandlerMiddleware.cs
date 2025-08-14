using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;
using ErrSendApplication.Interfaces.Telegram;

namespace ErrSendWebApi.ExceptionMidlevare
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITelegramService telegramService;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ITelegramService telegramService)
        {
            this.next = next;
            this.telegramService = telegramService;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var exStat = new ExecutionStatus { Status = "ER" };
            string additionalInfo = $"Path: {context.Request.Path}, Method: {context.Request.Method}, IP: {context.Connection.RemoteIpAddress}";

            switch (exception)
            {
                case SecurityTokenException:
                    code = HttpStatusCode.Unauthorized;
                    exStat.Errors.Add(exception.Message);
                    break;
                case InvalidOperationException:
                    code = HttpStatusCode.Conflict;
                    exStat.Errors.Add(exception.Message);
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    exStat.Errors.Add("Внутрішня помилка сервера");
                    await telegramService.SendErrorMessageAsync(exception.Message, additionalInfo);
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            var result = JsonSerializer.Serialize(exStat);
            await context.Response.WriteAsync(result);
        }
    }
}
