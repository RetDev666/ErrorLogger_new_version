using FluentValidation;

namespace ErrSendApplication.Validators
{
    public class EnvironmentConfigValidator : AbstractValidator<(string? envSecret, string configSecret, int configLength)>
    {
        public EnvironmentConfigValidator()
        {
            try
            {
                RuleFor(x => x.configSecret)
                    .NotEmpty().WithMessage("JWT Secret не може бути порожнім")
                    .MinimumLength(32).WithMessage("JWT Secret має бути довжиною не менше 32 символів");

                When(x => !string.IsNullOrEmpty(x.envSecret), () =>
                {
                    RuleFor(x => x.envSecret)
                        .MinimumLength(32).WithMessage("JWT Secret з змінної середовища має бути довжиною не менше 32 символів");
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації EnvironmentConfig: {ex.Message}", ex);
            }
        }
    }
}
