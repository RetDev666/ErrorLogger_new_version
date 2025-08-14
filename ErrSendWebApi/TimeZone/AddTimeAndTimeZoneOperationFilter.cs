using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using FluentValidation;
using ErrSendWebApi.Validators;

namespace ErrSendWebApi.TimeZone
{
    public class AddTimeAndTimeZoneOperationFilter : IOperationFilter
    {
        private readonly IValidator<(OpenApiOperation operation, string responseKey, string contentType)> validator;

        public AddTimeAndTimeZoneOperationFilter(IValidator<(OpenApiOperation operation, string responseKey, string contentType)> validator)
        {
            this.validator = validator;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Валідація параметрів через FluentValidation
            var validationResult = validator.Validate((operation, "200", "application/json"));
            if (!validationResult.IsValid)
            {
                return; // Виходимо, якщо валідація не пройшла
            }

            if (operation.Responses.ContainsKey("200"))
            {
                var response = operation.Responses["200"];
                
                if (response.Content != null && response.Content.ContainsKey("application/json"))
                {
                    var content = new OpenApiObject
                    {
                        ["time"] = new OpenApiString(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                        ["timezone"] = new OpenApiString(TimeZoneInfo.FindSystemTimeZoneById("Europe/Kiev").DisplayName)
                    };

                    response.Content["application/json"].Example = content;
                }
            }
        }
    }
}
