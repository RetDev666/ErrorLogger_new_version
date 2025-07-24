using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ErrSendWebApi.TimeZone
{
    public class AddTimeAndTimeZoneOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
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
