using BankApp.Client.Dto;
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

            _httpClient.Timeout = TimeSpan.FromMinutes(5);

        }

        private async Task<string> GetToken()

        {

            try

            {

                var jwtToken = _httpContextAccessor.HttpContext?.User.FindFirst("jwttoken")?.Value;

                if (!string.IsNullOrEmpty(jwtToken))

                {

                    return jwtToken;

                }

                string clientId = _configuration["ApiSetting:ClientId"];

                string clientSecret = _configuration["ApiSetting:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))

                {

                    return null;

                }

                var tokenRequest = new Dictionary<string, string>

                {

                    { "userName", clientId },

                    { "password", clientSecret }

                };

                var content = new StringContent(
     JsonSerializer.Serialize(tokenRequest),
     Encoding.UTF8,
     "application/json"
 );


                var request = new HttpRequestMessage(HttpMethod.Post, "Token/GetToken")

                {

                    Content = content

                };

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)

                {

                    string result = await response.Content.ReadAsStringAsync();

                    var tokenResponse = JsonSerializer.Deserialize<Result<string>>(result, new JsonSerializerOptions

                    {

                        PropertyNameCaseInsensitive = true

                    });

                    return tokenResponse?.Response;

                }

                return null;

            }

            catch

            {

                return null;

            }

        }
        private async Task AddAuthorizationHeader()

        {

            string token = await GetToken();

            if (!string.IsNullOrEmpty(token))

            {

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            }

        }

        public async Task<T> GetAsync<T>(string url)

        {

            await AddAuthorizationHeader();

            var response = await _httpClient.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {content}");

            }

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

        public async Task<T> PostAsync<T>(string url, object data)

        {

            await AddAuthorizationHeader();

            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseContent}");

            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

        public async Task<T> PostAsync<T>(string url)

        {

            await AddAuthorizationHeader();

            var response = await _httpClient.PostAsync(url, null);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseContent}");

            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

        public async Task<T> PostFormDataAsync<T>(string url, MultipartFormDataContent formData)

        {

            await AddAuthorizationHeader();

            var response = await _httpClient.PostAsync(url, formData);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseContent}");

            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

        public async Task<T> PutAsync<T>(string url, object data)

        {

            await AddAuthorizationHeader();

            var json = JsonSerializer.Serialize(data);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseContent}");

            }

            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

        public async Task<T> DeleteAsync<T>(string url)

        {

            await AddAuthorizationHeader();

            var response = await _httpClient.DeleteAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)

            {

                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {content}");

            }

            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions

            {

                PropertyNameCaseInsensitive = true

            });

        }

    }

}

