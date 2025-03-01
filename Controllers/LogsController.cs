using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CountryBlockingAPI.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly IBlockedAttemptsRepository _blockedAttemptsRepository;
    private readonly ILogger<LogsController> _logger;

    public LogsController(
        IBlockedAttemptsRepository blockedAttemptsRepository,
        ILogger<LogsController> logger)
    {
        _blockedAttemptsRepository = blockedAttemptsRepository;
        _logger = logger;
    }

    // GET: api/logs/blocked-attempts
    [HttpGet("blocked-attempts")]
    public async Task<IActionResult> GetBlockedAttempts([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
    {
        if (pageIndex < 1) pageIndex = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var attempts = await _blockedAttemptsRepository.GetBlockedAttemptsAsync(pageIndex, pageSize);
        return Ok(attempts);
    }
}