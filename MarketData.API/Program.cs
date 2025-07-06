
using AutoMapper;
using MarketData.BLL.Interfaces;
using MarketData.BLL.Models.Other;
using MarketData.BLL.Services;
using MarketData.DAL.Configurations;
using MarketData.DAL.Interfaces;
using MarketData.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MarketData.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Db connection
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration["ConnectionStrings:MarketDataDB"]);
                opt.EnableSensitiveDataLogging();
            });


            builder.Services.Configure<FintachartsSettings>(
                builder.Configuration.GetSection("Fintacharts"));

            //mapper config
            var mapperConfig = new MapperConfiguration(mc => { mc.AddProfile(new MapperProfile()); });

            var mapper = mapperConfig.CreateMapper();
            builder.Services.AddSingleton(mapper);

            builder.Services.AddSwaggerGen();

            // HttpClient
            builder.Services.AddHttpClient("ApiClient", (sp, client) =>
            {
                var baseUrl = builder.Configuration["Fintacharts:ApiUri"];
                client.BaseAddress = new Uri(baseUrl);
            });

            // Repositories
            builder.Services.AddScoped<IAssetRepository, AssetRepository>();
            

            // Services
            builder.Services.AddSingleton<IAuthService, AuthService>();
            builder.Services.AddScoped<IAssetService, AssetService>();
            builder.Services.AddSingleton<IPriceCacheService, PriceCacheService>();


            var app = builder.Build();

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
