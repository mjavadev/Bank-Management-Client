using BankApp.Client.HttpClients;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BankApp.Client.HttpClients
{
    public class GenericHttpClient : IGenericHttpClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public GenericHttpClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;

            var baseUrl = _configuration.GetValue<string>("ApiSetting:ClientUrl");
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        private void SetAuthorizationHeader()
        {
            var basicAuth = _httpContextAccessor.HttpContext?.User.FindFirst("basicauth")?.Value;

            if (!string.IsNullOrEmpty(basicAuth))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Basic", basicAuth);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<T> GetAsync<T>(string url)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {content}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string url, object data)
        {
            SetAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {responseContent}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {responseContent}", ex);
            }
        }

        public async Task<T> PostAsync<T>(string url)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsync(url, null);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {responseContent}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {responseContent}", ex);
            }
        }

        public async Task<T> PutAsync<T>(string url, object data)
        {
            SetAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {responseContent}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {responseContent}", ex);
            }
        }

        public async Task<T> DeleteAsync<T>(string url)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {content}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {content}", ex);
            }
        }

        public async Task<T> PostFormDataAsync<T>(string url, MultipartFormDataContent content)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PostAsync(url, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Check if response is successful
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API Error: {response.StatusCode} - {responseContent}");
            }

            // Check if content is empty
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new HttpRequestException("API returned empty response");
            }

            try
            {
                return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                throw new HttpRequestException($"Failed to deserialize response. Content: {responseContent}", ex);
            }
        }
    }


}
