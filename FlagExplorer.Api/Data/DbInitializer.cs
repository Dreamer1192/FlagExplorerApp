using FlagExplorer.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FlagExplorer.Api.Data
{
    public static class DbInitializer
    {
        public static async Task SeedCountriesAsync(CountryContext context, HttpClient httpClient)
        {
            // If there are already countries, don't seed.
            if (await context.Countries.AnyAsync())
            {
                return;
            }

            // Fetch data from the external API.
            var response = await httpClient.GetAsync("https://restcountries.com/v3.1/all");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();

            var countries = new List<Country>();

            using (JsonDocument doc = JsonDocument.Parse(json))
            {
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    // Extract the common name.
                    string name = element.GetProperty("name").GetProperty("common").GetString();

                    // Extract population (if available).
                    long population = element.TryGetProperty("population", out JsonElement popElement)
                        ? popElement.GetInt64()
                        : 0;

                    // Extract the capital. The API returns an array; take the first value if available.
                    string capital = "";
                    if (element.TryGetProperty("capital", out JsonElement capitalElement) &&
                        capitalElement.ValueKind == JsonValueKind.Array &&
                        capitalElement.GetArrayLength() > 0)
                    {
                        capital = capitalElement[0].GetString();
                    }

                    // Updated flag logic: prefer the URL from the "flags" object.
                    string flag = "";
                    if (element.TryGetProperty("flags", out JsonElement flagsElement))
                    {
                        // Prefer PNG image if available.
                        if (flagsElement.TryGetProperty("png", out JsonElement flagPng))
                        {
                            flag = flagPng.GetString();
                        }
                        else if (flagsElement.TryGetProperty("svg", out JsonElement flagSvg))
                        {
                            flag = flagSvg.GetString();
                        }
                    }
                    else if (element.TryGetProperty("flag", out JsonElement flagElement))
                    {
                        // Fallback if the "flags" object is missing (though ideally, it should exist).
                        flag = flagElement.GetString();
                    }

                    // Create a new Country object.
                    var country = new Country
                    {
                        Name = name,
                        Population = population,
                        Capital = capital,
                        Flag = flag // This will now be a URL to the image.
                    };

                    countries.Add(country);
                }
            }

            // Add the fetched countries to the context and save.
            await context.Countries.AddRangeAsync(countries);
            await context.SaveChangesAsync();
        }
    }
}
