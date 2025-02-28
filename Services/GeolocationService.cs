/*separating API Logic in a dedicated service*/
using System.Net.Http.Json;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

// responsible for calling the 3rd party geoService api (ipapi.co) to fetch country info based on IP address
namespace CountryBlockingAPI.Services;

public class GeolocationService : IGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeolocationService> _logger;
    private readonly bool _isTestMode;

    // dependency injection
    public GeolocationService(HttpClient httpClient, ILogger<GeolocationService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _isTestMode = configuration.GetValue<bool>("GeolocationApi:TestMode");
    }

    public async Task<CountryInfo?> GetCountryInfoByIpAsync(string ipAddress)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                _logger.LogWarning("Invalid IP address provided");
                return null;
            }

            // this is for test mode, return mock data "just for testing purposes"
            if (_isTestMode)
            {
                return new CountryInfo 
                { 
                    Ip = ipAddress,
                    CountryCode = "US",
                    Country = "United States",
                    City = "Test City"
                };
            }

            // Add delay to respect rate limiting (1 request per second for free tier according to ipapi.co docs)
            await Task.Delay(1000);

            var response = await _httpClient.GetAsync($"{ipAddress}/json/");

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _logger.LogWarning("Rate limit exceeded for IP {IpAddress}", ipAddress);
                // Return a basic response instead of null
                return new CountryInfo 
                { 
                    Ip = ipAddress,
                    CountryCode = "US",
                    Country = "United States"
                };
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get country info for IP {IpAddress}. Status code: {StatusCode}", 
                    ipAddress, response.StatusCode);
                return null;
            }

            var countryInfo = await response.Content.ReadFromJsonAsync<CountryInfo>();

            // check if the response contains an error
            if(countryInfo?.Error == true)
            {
                _logger.LogWarning("Error from ipapi.co for IP {IpAddress}: {Reason}", 
                    ipAddress, countryInfo.Reason);
                return null;
            }

            return countryInfo;
        } 
        catch(Exception ex) 
        {
            _logger.LogError(ex, "Error getting country info for IP {IpAddress}", ipAddress);
            return null;
        }
    }
}
