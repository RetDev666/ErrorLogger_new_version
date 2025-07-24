using System.Net.Http.Headers;
using ErrSendApplication.Interfaces.Client;

namespace ErrSendPersistensTelegram.Services
{
    public class StandartHttpClient : IHttpClientWr
    {
        private readonly HttpClient httpClient;
        private bool disposed = false;

        public StandartHttpClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.httpClient.Timeout = TimeSpan.FromMinutes(180);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, string? token = null)
        {
            if (token is not null)
            {
                if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
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
