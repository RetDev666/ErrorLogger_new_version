using System.Net.Http.Headers;
using ErrSendApplication.Interfaces.Client;
using FluentValidation;

namespace ErrSendPersistensTelegram.Services
{
    public class StandartHttpClient : IHttpClientWr
    {
        private readonly HttpClient httpClient;
        private readonly IValidator<(string url, HttpContent content, string? token)> validator;
        private bool disposed = false;

        public StandartHttpClient(HttpClient httpClient, IValidator<(string url, HttpContent content, string? token)> validator)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.validator = validator;
            this.httpClient.Timeout = TimeSpan.FromMinutes(180);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, string? token = null)
        {
            // Валідація параметрів через FluentValidation
            var validationResult = await validator.ValidateAsync((url, content, token));
            if (!validationResult.IsValid)
            {
                var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException($"Параметри HTTP запиту невалідні: {errors}");
            }

            if (token is not null)
            {
                if (httpClient.DefaultRequestHeaders.Contains("Авторизація"))
                {
                    httpClient.DefaultRequestHeaders.Remove("Авторизація");
                }
                httpClient.DefaultRequestHeaders.Add("Авторизація", "Носій " + token);
            }
            
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            return await httpClient.PostAsync(url, content);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    httpClient?.Dispose();
                }
                disposed = true;
            }
        }
    }
}
