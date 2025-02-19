using System;
using System.Threading.Tasks;

namespace FlagExplorer.Api.Services.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T item, TimeSpan expiration);
        Task RemoveAsync(string key);
    }
}
