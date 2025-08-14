using FluentValidation;
using ErrSendApplication.DTO;

namespace ErrSendApplication.Validators
{
    public class ErrorRequestValidator : AbstractValidator<ErrorRequest>
    {
        public ErrorRequestValidator()
        {
            try
            {
                RuleFor(x => x.ErrorMessage)
                    .NotEmpty().WithMessage("Повідомлення про помилку не повинно бути порожнім");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації ErrorRequest: {ex.Message}", ex);
            }
        }
    }
} 