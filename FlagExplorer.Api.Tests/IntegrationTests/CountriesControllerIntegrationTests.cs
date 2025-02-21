using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FlagExplorer.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FlagExplorer.Api.Tests.IntegrationTests
{
    public class CountriesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CountriesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Create an HttpClient to send requests to the in-memory test server.
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllCountries_ReturnsSuccessAndData()
        {
            // Arrange
            var requestUrl = "/api/Countries";

            // Act
            var response = await _client.GetAsync(requestUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var countries = JsonSerializer.Deserialize<List<Country>>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(countries);
            Assert.NotEmpty(countries);
        }

        [Fact]
        public async Task GetCountryByName_ReturnsNotFound_ForInvalidCountry()
        {
            // Arrange
            var invalidCountryName = "InvalidCountry";
            var requestUrl = $"/api/Countries/{invalidCountryName}";

            // Act
            var response = await _client.GetAsync(requestUrl);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetCountryByName_ReturnsOk_WithExpectedCountry()
        {
            // Arrange: Ensure the seeded data contains a known country (e.g., "France").
            var requestUrl = "/api/Countries/France";

            // Act
            var response = await _client.GetAsync(requestUrl);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var country = JsonSerializer.Deserialize<Country>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(country);
            Assert.Equal("France", country.Name);
        }
    }
}
