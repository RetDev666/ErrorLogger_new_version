using FluentValidation;
using ErrSendApplication.DTO;

namespace ErrSendApplication.Validators
{
    public class ErrorRequestValidator : AbstractValidator<ErrorRequest>
    {
        public ErrorRequestValidator()
        {
            RuleFor(x => x.ErrorMessage)
                .NotEmpty().WithMessage("ErrorMessage must not be empty");
        }
    }
} 