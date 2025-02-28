using System.ComponentModel.DataAnnotations
using CountryBlockingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using CountryBlockingAPI.Interfaces;

namespace CountryBlockingAPI.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly IBlockedCountryRepository _blockedCountryRepository;
    private readonly ITemporalBlockRepository _temporalBlockRepository;
    private readonly IGeolocationService _geolocationService;
    private readonly ILogger<CountriesController> _logger;

    public CountriesController(IBlockedCountryRepository blockedCountryRepository,
        ITemporalBlockRepository temporalBlockRepository,
        IGeolocationService geolocationService,
        ILogger<CountriesController> logger)
    {
        _blockedCountryRepository = blockedCountryRepository;
        _temporalBlockRepository = temporalBlockRepository;
        _geolocationService = geolocationService;
        _logger = logger;
    }
    
    // this is the POST REQUEST => api/countries/block
    [HttpPost("block")]
    public async Task<IActionResult> BlockCountry([FromBody] BlockCountryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
        {
            return BadRequest("hey wait, country code is required here...");
        }

        var countryCode = request.CountryCode.ToUpperInvariant();

        if (await _blockedCountryRepository.IsCountryBlockedAsync(countryCode))
        {
            return Conflict($"this country of code {countryCode} is already blocked");
        }

    }
    
    
    
}

