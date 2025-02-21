using FlagExplorer.Api.Data;
using FlagExplorer.Api.Models;
using FlagExplorer.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlagExplorer.Api.Services
{
    public class CountryService : ICountryService
    {
        private readonly CountryContext _context;
        private readonly ILogger<CountryService> _logger;

        public CountryService(CountryContext context, ILogger<CountryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Country>> GetAllCountriesAsync()
        {
            try
            {
                var countries = await _context.Countries.ToListAsync();
                return countries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all countries.");
                throw;
            }
        }

        public async Task<Country?> GetCountryByNameAsync(string name)
        {
            try
            {
                var country = await _context.Countries
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());

                if (country == null)
                {
                    _logger.LogWarning("Country with name {Name} not found.", name);
                }

                return country;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the country with name {Name}.", name);
                throw;
            }
        }
    }
}
