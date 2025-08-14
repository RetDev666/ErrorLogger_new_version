using FluentValidation;
using ErrSendApplication.Common.Configs;

namespace ErrSendApplication.Validators
{
    public class JwtConfigValidator : AbstractValidator<JwtConfig>
    {
        public JwtConfigValidator()
        {
            try
            {
                RuleFor(x => x.Secret)
                    .NotEmpty().WithMessage("JWT Secret не може бути порожнім")
                    .MinimumLength(32).WithMessage("JWT Secret має бути довжиною не менше 32 символів")
                    .MaximumLength(512).WithMessage("JWT Secret занадто довгий (максимум 512 символів)");

                RuleFor(x => x.ExpiryMinutes)
                    .GreaterThan(0).WithMessage("Час життя токена має бути більше 0 хвилин")
                    .LessThanOrEqualTo(1440).WithMessage("Час життя токена не може перевищувати 24 години (1440 хвилин)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації JwtConfig: {ex.Message}", ex);
            }
        }
    }
}
