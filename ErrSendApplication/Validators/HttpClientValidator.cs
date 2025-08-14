using FluentValidation;

namespace ErrSendApplication.Validators
{
    public class HttpClientValidator : AbstractValidator<(string url, HttpContent content, string? token)>
    {
        public HttpClientValidator()
        {
            try
            {
                RuleFor(x => x.url)
                    .NotEmpty().WithMessage("URL не може бути порожнім")
                    .Must(BeValidUrl).WithMessage("URL має бути валідним");

                RuleFor(x => x.content)
                    .NotNull().WithMessage("Content не може бути null");

                When(x => !string.IsNullOrEmpty(x.token), () =>
                {
                    RuleFor(x => x.token)
                        .MinimumLength(10).WithMessage("Token має бути довжиною не менше 10 символів")
                        .MaximumLength(1000).WithMessage("Token занадто довгий (максимум 1000 символів)");
                });
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Помилка створення правил валідації HttpClient: {ex.Message}", ex);
            }
        }

        private bool BeValidUrl(string url)
        {
            try
            {
                return Uri.TryCreate(url, UriKind.Absolute, out _);
            }
            catch
            {
                return false;
            }
        }
    }
}
