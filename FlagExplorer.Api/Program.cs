using FlagExplorer.Api.Services.Interfaces;
using FlagExplorer.Api.Services;
using FlagExplorer.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace FlagExplorer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add controllers
            builder.Services.AddControllers();

            // Swagger/OpenAPI configuration
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register EF Core In-Memory Database for CountryContext.
            builder.Services.AddDbContext<CountryContext>(options =>
                options.UseInMemoryDatabase("CountriesDb"));

            // Register CountryService with its interface.
            builder.Services.AddScoped<ICountryService, CountryService>();
            //builder.Services.AddScoped<ICacheService, RedisCacheService>();

            var app = builder.Build();

            // Ensure the in-memory database is created and seeded.
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CountryContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
