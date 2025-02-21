using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlagExplorer.Api.Models;
using FlagExplorer.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlagExplorer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly ILogger<CountriesController> _logger;

        public CountriesController(ICountryService countryService, ILogger<CountriesController> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/Countries
        /// Retrieves all countries.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetAllCountries()
        {
            try
            {
                var countries = await _countryService.GetAllCountriesAsync();
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries.");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// GET: api/Countries/{name}
        /// Retrieves a specific country by its name.
        /// </summary>
        [HttpGet("{name}")]
        public async Task<ActionResult<Country>> GetCountryByName(string name)
        {
            try
            {
                var country = await _countryService.GetCountryByNameAsync(name);
                if (country == null)
                {
                    return NotFound();
                }
                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country with name {Name}.", name);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
