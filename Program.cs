using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Services;
using CountryBlockingAPI.Repositories;
using CountryBlockingAPI.Models;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Country Blocking API", Version = "v1" });
});

// the following lines register repos as singletons (in-memory storage)
builder.Services.AddSingleton<IBlockedCountryRepository, BlockedCountryRepository>();
builder.Services.AddSingleton<ITemporalBlockRepository, TemporalBlockRepository>();
builder.Services.AddSingleton<IBlockedAttemptsRepository, BlockedAttemptsRepository>();

// register http client for geolocation service 
builder.Services.AddHttpClient<IGeolocationService, GeolocationService>(client =>
{
    var baseUrl = builder.Configuration["GeolocationApi:BaseUrl"];
    client.BaseAddress = new Uri(baseUrl ?? "https://ipapi.co/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Country Blocking API v1"));
}

app.UseHttpsRedirection();

// Add routing middleware
app.UseRouting();

app.UseAuthorization();

// Add endpoint middleware
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
