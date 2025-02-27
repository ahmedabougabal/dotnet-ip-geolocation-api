/*separating API Logic in a dedicated service*/
using System.Net.Http.Json;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.Extensions.Options;


// responsible for calling the 3rd party geoService api (ipapi.co) to fetch country info based on IP address
namespace CountryBlockingAPI.Services;

public class GeoLocationService : IGeoLocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeoLocationService> _logger;

    // dependency injection
    public GeoLocationService(HttpClient httpClient, ILogger<GeoLocationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }



}

