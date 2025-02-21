using FlagExplorer.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlagExplorer.Api.Services.Interfaces
{
    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetAllCountriesAsync();
        Task<Country?> GetCountryByNameAsync(string name);
    }
}
