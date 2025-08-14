using FluentValidation;
using System.Net;

namespace ErrSendWebApi.Validators
{
    public class ExceptionHandlerValidator : AbstractValidator<(HttpContext context, Exception exception, HttpStatusCode statusCode)>
    {
        public ExceptionHandlerValidator()
        {
            try
            {
                RuleFor(x => x.context)
                    .NotNull().WithMessage("HttpContext не може бути null");

                RuleFor(x => x.exception)
                    .NotNull().WithMessage("Exception не може бути null");

                RuleFor(x => x.statusCode)
                    .IsInEnum().WithMessage("Status code має бути валідним HTTP статусом");

                RuleFor(x => x.context.Request.Path)
                    .NotEmpty().WithMessage("Request Path не може бути порожнім");

                RuleFor(x => x.context.Request.Method)
                    .NotEmpty().WithMessage("Request Method не може бути порожнім");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації ExceptionHandler: {ex.Message}", ex);
            }
        }
    }
}
