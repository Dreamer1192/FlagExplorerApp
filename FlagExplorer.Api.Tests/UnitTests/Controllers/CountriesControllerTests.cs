using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using FlagExplorer.Api.Controllers;
using FlagExplorer.Api.Models;
using FlagExplorer.Api.Services.Interfaces;

namespace FlagExplorer.Api.Tests.UnitTests.Controllers
{
    public class CountriesControllerTests
    {
        [Fact]
        public async Task GetAllCountries_ReturnsOkResult_WithCountries()
        {
            // Arrange: Create a fake list of countries.
            var fakeCountries = new List<Country>
            {
                new Country { Id = 1, Name = "France", Flag = "🇫🇷", Population = 67000000, Capital = "Paris" },
                new Country { Id = 2, Name = "Brazil", Flag = "🇧🇷", Population = 210000000, Capital = "Brasília" }
            };

            var mockCountryService = new Mock<ICountryService>();
            mockCountryService.Setup(s => s.GetAllCountriesAsync())
                              .ReturnsAsync(fakeCountries);

            var mockLogger = new Mock<ILogger<CountriesController>>();
            var controller = new CountriesController(mockCountryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllCountries();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCountries = Assert.IsAssignableFrom<IEnumerable<Country>>(okResult.Value);
            Assert.Equal(fakeCountries.Count, returnedCountries.Count());
        }

        [Fact]
        public async Task GetCountryByName_ReturnsNotFound_WhenCountryDoesNotExist()
        {
            // Arrange: Setup service to return null for an unknown country.
            var mockCountryService = new Mock<ICountryService>();
            mockCountryService.Setup(s => s.GetCountryByNameAsync("NonExistent"))
                              .ReturnsAsync((Country)null);

            var mockLogger = new Mock<ILogger<CountriesController>>();
            var controller = new CountriesController(mockCountryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetCountryByName("NonExistent");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetCountryByName_ReturnsOkResult_WithCountry()
        {
            // Arrange: Setup a valid country.
            var expectedCountry = new Country { Id = 3, Name = "Japan", Flag = "🇯🇵", Population = 125800000, Capital = "Tokyo" };

            var mockCountryService = new Mock<ICountryService>();
            mockCountryService.Setup(s => s.GetCountryByNameAsync("Japan"))
                              .ReturnsAsync(expectedCountry);

            var mockLogger = new Mock<ILogger<CountriesController>>();
            var controller = new CountriesController(mockCountryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetCountryByName("Japan");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCountry = Assert.IsType<Country>(okResult.Value);
            Assert.Equal(expectedCountry.Id, returnedCountry.Id);
            Assert.Equal(expectedCountry.Name, returnedCountry.Name);
        }

        [Fact]
        public async Task GetAllCountries_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange: Setup service to throw an exception.
            var mockCountryService = new Mock<ICountryService>();
            mockCountryService.Setup(s => s.GetAllCountriesAsync())
                              .ThrowsAsync(new Exception("Database error"));

            var mockLogger = new Mock<ILogger<CountriesController>>();
            var controller = new CountriesController(mockCountryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetAllCountries();

            // Assert: Verify an ObjectResult is returned with status code 500.
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetCountryByName_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange: Setup service to throw an exception for any country name.
            var mockCountryService = new Mock<ICountryService>();
            mockCountryService.Setup(s => s.GetCountryByNameAsync(It.IsAny<string>()))
                              .ThrowsAsync(new Exception("Database error"));

            var mockLogger = new Mock<ILogger<CountriesController>>();
            var controller = new CountriesController(mockCountryService.Object, mockLogger.Object);

            // Act
            var result = await controller.GetCountryByName("France");

            // Assert: Verify an ObjectResult is returned with status code 500.
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}
