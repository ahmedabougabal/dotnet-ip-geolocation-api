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
    public async Task<IActionResult> BlockCountry([FromBody] BlockCountryRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CountryCode))
            {
                return BadRequest("Country code is required");
            }

            // Normalize country code
            request.CountryCode = request.CountryCode.ToUpperInvariant();

            // Validate country code format (must be exactly 2 letters)
            if (!IsValidCountryCode(request.CountryCode))
            {
                return BadRequest($"Invalid country code: {request.CountryCode}. Country code must be a valid ISO 3166-1 alpha-2 code.");
            }

            // Check if country is already blocked
            if (await _blockedCountryRepository.IsCountryBlockedAsync(request.CountryCode))
            {
                return Conflict($"Country {request.CountryCode} is already blocked");
            }

            // Create a default CountryInfo object with the country code
            var countryInfo = new CountryInfo
            {
                CountryCode = request.CountryCode,
                CountryName = request.CountryCode // Use code as name initially, can be updated later
            };

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

        // Validate country code format (must be exactly 2 letters)
        if (!IsValidCountryCode(countryCode))
        {
            return BadRequest($"Invalid country code: {countryCode}. Country code must be a valid ISO 3166-1 alpha-2 code.");
        }

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
    public async Task<IActionResult> GetBlockedCountries(
        [FromQuery] int pageIndex = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var blockedCountries = await _blockedCountryRepository.GetBlockedCountriesAsync(pageIndex, pageSize, searchTerm);
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

        // Validate country code format (must be exactly 2 letters)
        if (!IsValidCountryCode(request.CountryCode))
        {
            return BadRequest($"Invalid country code: {request.CountryCode}. Country code must be a valid ISO 3166-1 alpha-2 code.");
        }

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
            expirationTime = temporalBlock.ExpirationTime
        });
    }

    // Helper method to validate country codes
    private bool IsValidCountryCode(string countryCode)
    {
        // Check if the country code is exactly 2 uppercase letters
        if (string.IsNullOrEmpty(countryCode) || countryCode.Length != 2)
        {
            return false;
        }

        // Check if the country code consists only of letters
        foreach (char c in countryCode)
        {
            if (!char.IsLetter(c))
            {
                return false;
            }
        }

        // List of invalid/example country codes that should be rejected
        var invalidCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "XX", // Commonly used as example/placeholder
            "ZZ", // Commonly used as example/placeholder
            "XY", // Not assigned
            "XZ", // Not assigned
            "YZ", // Not assigned
            "YY", // Not assigned
            "ZX", // Not assigned
            "ZY"  // Not assigned
        };

        return !invalidCodes.Contains(countryCode);
    }
}
