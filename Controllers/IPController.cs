using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CountryBlockingAPI.Controllers;

[ApiController]
[Route("api/ip")]
public class IPController : ControllerBase
{
    private readonly IBlockedCountryRepository _blockedCountryRepository;
    private readonly ITemporalBlockRepository _temporalBlockRepository;
    private readonly IGeolocationService _geolocationService;
    private readonly IBlockedAttemptsRepository _blockedAttemptsRepository;
    private readonly ILogger<IPController> _logger;

    public IPController(
        IBlockedCountryRepository blockedCountryRepository,
        ITemporalBlockRepository temporalBlockRepository,
        IGeolocationService geolocationService,
        IBlockedAttemptsRepository blockedAttemptsRepository,
        ILogger<IPController> logger)
    {
        _blockedCountryRepository = blockedCountryRepository;
        _temporalBlockRepository = temporalBlockRepository;
        _geolocationService = geolocationService;
        _blockedAttemptsRepository = blockedAttemptsRepository;
        _logger = logger;
    }

    // GET: api/ip/lookup
    [HttpGet("lookup")]
    public async Task<IActionResult> LookupCountry([FromQuery] string? ipAddress = null)
    {
        // If no IP provided, use the caller's IP
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return BadRequest("Could not determine IP address");
            }
        }

        var countryInfo = await _geolocationService.GetCountryInfoByIpAsync(ipAddress);
        if (countryInfo == null)
        {
            return NotFound($"Could not find country information for IP {ipAddress}");
        }

        return Ok(countryInfo);
    }

    // GET: api/ip/check-block
    [HttpGet("check-block")]
    public async Task<IActionResult> CheckBlock()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return BadRequest("Could not determine IP address");
        }
        /*
        If running locally (localhost/127.0.0.1/::1), I will use a default public IP for testing
        because it is not possible to get an external public IP from localhost as asked in the assignment*/
        // Check if running locally
        bool isRunningLocally = ipAddress == "::1" || ipAddress == "127.0.0.1" || ipAddress == "localhost";
        string originalIpAddress = ipAddress;

        // If running locally, use a default public IP for testing
        if (isRunningLocally)
        {
            // Use a default public IP for testing (Google DNS)
            ipAddress = "8.8.8.8";
            _logger.LogInformation("Using default public IP {IpAddress} for local testing", ipAddress);
        }

        var countryInfo = await _geolocationService.GetCountryInfoByIpAsync(ipAddress);
        if (countryInfo == null)
        {
            return NotFound($"Could not find country information for IP {ipAddress}");
        }

        var userAgent = Request.Headers.UserAgent.ToString();

        var blockedAttempt = new BlockedAttempt
        {
            IpAddress = ipAddress,
            CountryCode = countryInfo.CountryCode ?? string.Empty,
            UserAgent = userAgent,
            IsBlocked = false
        };

        // Check if country is blocked (either permanently or temporarily)
        if (!string.IsNullOrEmpty(countryInfo.CountryCode))
        {
            blockedAttempt.IsBlocked = 
                await _blockedCountryRepository.IsCountryBlockedAsync(countryInfo.CountryCode) ||
                await _temporalBlockRepository.IsCountryTemporallyBlockedAsync(countryInfo.CountryCode);
        }

        // Log the attempt
        await _blockedAttemptsRepository.AddBlockedAttemptAsync(blockedAttempt);

        // Create the response object with additional information for local testing
        var response = new
        {
            ipAddress = ipAddress,
            countryCode = countryInfo.CountryCode,
            countryName = countryInfo.Country,
            isBlocked = blockedAttempt.IsBlocked
        };

        // i have added this additional information for local testing
        if (isRunningLocally)
        {
            return Ok(new
            {
                message = "You are running locally and it is not possible to fetch your real external IP address. Using a default IP address for testing purposes.",
                localIpAddress = originalIpAddress,
                testIpAddress = ipAddress,
                countryCode = countryInfo.CountryCode,
                countryName = countryInfo.Country,
                isBlocked = blockedAttempt.IsBlocked
            });
        }

        return Ok(response);
    }
}