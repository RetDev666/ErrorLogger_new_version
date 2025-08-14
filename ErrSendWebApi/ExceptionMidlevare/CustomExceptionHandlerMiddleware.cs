using Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;
using ErrSendApplication.Interfaces.Telegram;
using FluentValidation;
using ErrSendWebApi.Validators;

namespace ErrSendWebApi.ExceptionMidlevare
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ITelegramService telegramService;
        private readonly IValidator<(HttpContext context, Exception exception, HttpStatusCode statusCode)> validator;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ITelegramService telegramService, 
            IValidator<(HttpContext context, Exception exception, HttpStatusCode statusCode)> validator)
        {
            this.next = next;
            this.telegramService = telegramService;
            this.validator = validator;
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

            // Валідація параметрів через FluentValidation
            var validationResult = validator.Validate((context, exception, code));
            if (!validationResult.IsValid)
            {
                // Логуємо помилки валідації
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                code = HttpStatusCode.InternalServerError;
                exStat.Errors.Add($"Помилка валідації: {errors}");
            }
            else
            {
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
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            var result = JsonSerializer.Serialize(exStat);
            await context.Response.WriteAsync(result);
        }
    }
}
