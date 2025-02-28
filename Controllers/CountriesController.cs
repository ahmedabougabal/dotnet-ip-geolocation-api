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

    public CountriesController(
        IBlockedCountryRepository blockedCountryRepository,
        ITemporalBlockRepository temporalBlockRepository,
        IGeolocationService geolocationService,
        ILogger<CountriesController> logger)
    {
        _blockedCountryRepository = blockedCountryRepository;
        _temporalBlockRepository = temporalBlockRepository;
        _geolocationService = geolocationService;
        _logger = logger;
    }

    // POST: api/countries/block
    [HttpPost("block")]
    public async Task<IActionResult> BlockCountry([FromBody] BlockedAttempt request)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
        {
            return BadRequest("Country code is required");
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
            // If we can't get country info, create a minimal one with just the code
            countryInfo = new CountryInfo { CountryCode = request.CountryCode };
        }

        // Add to blocked countries
        var success = await _blockedCountryRepository.AddBlockedCountryAsync(request.CountryCode, countryInfo);
        if (!success)
        {
            return StatusCode(500, "Failed to block country");
        }

        _logger.LogInformation("Country {CountryCode} has been blocked", request.CountryCode);
        return Ok(new { message = $"Country {request.CountryCode} has been blocked" });
    }
}
