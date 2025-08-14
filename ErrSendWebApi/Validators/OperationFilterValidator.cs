using FluentValidation;
using Microsoft.OpenApi.Models;

namespace ErrSendWebApi.Validators
{
    public class OperationFilterValidator : AbstractValidator<(OpenApiOperation operation, string responseKey, string contentType)>
    {
        public OperationFilterValidator()
        {
            try
            {
                RuleFor(x => x.operation)
                    .NotNull().WithMessage("Operation не може бути null");

                RuleFor(x => x.responseKey)
                    .NotEmpty().WithMessage("Response key не може бути порожнім");

                RuleFor(x => x.contentType)
                    .NotEmpty().WithMessage("Content type не може бути порожнім")
                    .Must(BeValidContentType).WithMessage("Content type має бути валідним MIME типом");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації OperationFilter: {ex.Message}", ex);
            }
        }

        private bool BeValidContentType(string contentType)
        {
            try
            {
                return !string.IsNullOrEmpty(contentType) && contentType.Contains("/");
            }
            catch
            {
                return false;
            }
        }
    }
}
