namespace BankApp.Client.HttpClients
{
    public interface IGenericHttpClient
    {
        Task<T> GetAsync<T>(string url);
        Task<T> PostAsync<T>(string url, object data);
        Task<T> PutAsync<T>(string url, object data);
        Task<T> DeleteAsync<T>(string url);
        Task<T> PostFormDataAsync<T>(string url, MultipartFormDataContent content);
        Task<T> PostAsync<T>(string url);
    }
}
