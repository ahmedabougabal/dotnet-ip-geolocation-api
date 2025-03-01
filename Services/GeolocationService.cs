using System.Net.Http.Json;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CountryBlockingAPI.Services;

public class GeolocationService : IGeolocationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeolocationService> _logger;
    private readonly string _userAgent;
    private static DateTime _lastRequestTime = DateTime.MinValue;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public GeolocationService(HttpClient httpClient, ILogger<GeolocationService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userAgent = "CountryBlockingAPI/1.0";

        // Set default headers
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _userAgent);
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

            // Respect rate limits - only 1 request per second
            await _semaphore.WaitAsync();
            try
            {
                var timeSinceLastRequest = DateTime.UtcNow - _lastRequestTime;
                if (timeSinceLastRequest.TotalMilliseconds < 1000)
                {
                    var delayMs = 1000 - (int)timeSinceLastRequest.TotalMilliseconds;
                    if (delayMs > 0)
                    {
                        await Task.Delay(delayMs);
                    }
                }

                // Make the API call
                _lastRequestTime = DateTime.UtcNow;
                var response = await _httpClient.GetAsync($"{ipAddress}/json/");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get country info for IP {IpAddress}. Status code: {StatusCode}", 
                        ipAddress, response.StatusCode);
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: {Response}", responseContent);

                var countryInfo = await response.Content.ReadFromJsonAsync<CountryInfo>();
                if (countryInfo == null)
                {
                    _logger.LogWarning("Failed to parse country info for IP {IpAddress}", ipAddress);
                    return null;
                }

                // Check if the response indicates an error
                if (countryInfo.Error)
                {
                    _logger.LogWarning("Error from ipapi.co for IP {IpAddress}: {Reason}", 
                        ipAddress, countryInfo.Reason);
                    return null;
                }

                return countryInfo;
            }
            finally
            {
                _semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting country info for IP {IpAddress}", ipAddress);
            return null;
        }
    }
}
