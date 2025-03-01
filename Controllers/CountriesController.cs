using System.ComponentModel.DataAnnotations;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CountryBlockingAPI.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly IBlockedCountryRepository _blockedCountryRepository;
    private readonly ITemporalBlockRepository _temporalBlockRepository;
    private readonly IGeolocationService _geolocationService;
    private readonly ILogger<CountriesController> _logger;
    private readonly IBlockedAttemptsRepository _blockedAttemptsRepository;

    public CountriesController(
        IBlockedCountryRepository blockedCountryRepository,
        ITemporalBlockRepository temporalBlockRepository,
        IGeolocationService geolocationService,
        IBlockedAttemptsRepository blockedAttemptsRepository,
        ILogger<CountriesController> logger)
    {
        _blockedCountryRepository = blockedCountryRepository;
        _temporalBlockRepository = temporalBlockRepository;
        _geolocationService = geolocationService;
        _blockedAttemptsRepository = blockedAttemptsRepository;
        _logger = logger;
    }

    // POST: api/countries/block
    [HttpPost("block")]
    public async Task<IActionResult> BlockCountry([FromBody] BlockedAttempt request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
            {
                return BadRequest("Country code is required");
            }

            // Use a valid IP address for testing if none provided
            if (string.IsNullOrWhiteSpace(request.IpAddress))
            {
                request.IpAddress = "8.8.8.8"; // Google's DNS server (US)
            }

            // Normalize country code
            request.CountryCode = request.CountryCode.ToUpperInvariant();

            // Check if country is already blocked
            if (await _blockedCountryRepository.IsCountryBlockedAsync(request.CountryCode))
            {
                return Conflict($"Country {request.CountryCode} is already blocked");
            }

            // Get country info from geolocation service
            var countryInfo = await _geolocationService.GetCountryInfoByIpAsync(request.IpAddress);
            if (countryInfo == null)
            {
                return BadRequest($"Could not get country information for IP {request.IpAddress}. Please check the IP address and try again.");
            }

            // Make sure the country code in the country info matches the requested code
            countryInfo.CountryCode = request.CountryCode;
            
            // Add to blocked countries
            var success = await _blockedCountryRepository.AddBlockedCountryAsync(request.CountryCode, countryInfo);
            if (!success)
            {
                return StatusCode(500, "Failed to block country");
            }

            _logger.LogInformation("Country {CountryCode} has been blocked", request.CountryCode);
            return Ok(new { message = $"Country {request.CountryCode} has been blocked" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to block country {CountryCode}", request.CountryCode);
            return StatusCode(500, $"An error occurred while blocking the country: {ex.Message}");
        }
    }

    // DELETE: api/countries/block/{countryCode}
    [HttpDelete("block/{countryCode}")]
    public async Task<IActionResult> UnblockCountry([FromRoute] string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
        {
            return BadRequest("Country code is required");
        }

        countryCode = countryCode.ToUpperInvariant();

        if (!await _blockedCountryRepository.IsCountryBlockedAsync(countryCode))
        {
            return NotFound($"Country {countryCode} is not blocked");
        }

        var success = await _blockedCountryRepository.RemoveBlockedCountryAsync(countryCode);
        if (!success)
        {
            return StatusCode(500, "Failed to unblock country");
        }

        _logger.LogInformation("Country {CountryCode} has been unblocked", countryCode);
        return Ok(new { message = $"Country {countryCode} has been unblocked" });
    }

    // GET: api/countries/blocked
    [HttpGet("blocked")]
    public async Task<IActionResult> GetBlockedCountries([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var blockedCountries = await _blockedCountryRepository.GetBlockedCountriesAsync(pageIndex, pageSize);
        return Ok(blockedCountries);
    }

    // POST: api/countries/temporal-block
    [HttpPost("temporal-block")]
    public async Task<IActionResult> TemporalBlockCountry([FromBody] TemporalBlockRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
        {
            return BadRequest("Country code is required");
        }

        if (request.DurationMinutes < 1 || request.DurationMinutes > 1440)
        {
            return BadRequest("Duration must be between 1 and 1440 minutes");
        }

        request.CountryCode = request.CountryCode.ToUpperInvariant();

        // Check if country is already temporarily blocked
        if (await _temporalBlockRepository.IsCountryTemporallyBlockedAsync(request.CountryCode))
        {
            return Conflict($"Country {request.CountryCode} is already temporarily blocked");
        }

        var temporalBlock = new TemporalBlock
        {
            CountryCode = request.CountryCode,
            ExpirationTime = DateTime.UtcNow.AddMinutes(request.DurationMinutes)
        };

        var success = await _temporalBlockRepository.AddTemporalBlockAsync(temporalBlock);
        if (!success)
        {
            return StatusCode(500, "Failed to add temporal block");
        }

        _logger.LogInformation("Country {CountryCode} has been temporarily blocked for {Duration} minutes", 
            request.CountryCode, request.DurationMinutes);
        
        return Ok(new { 
            message = $"Country {request.CountryCode} has been temporarily blocked for {request.DurationMinutes} minutes",
            expiresAt = temporalBlock.ExpirationTime
        });
    }
}
