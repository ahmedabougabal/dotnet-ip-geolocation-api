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
        
        // If there are no attempts, add a sample attempt for testing
        if (attempts.Items.Count == 0 && attempts.TotalCount == 0)
        {
            _logger.LogInformation("No blocked attempts found. Adding a sample attempt for testing.");
            
            // Create a sample blocked attempt
            var sampleAttempt = new BlockedAttempt
            {
                IpAddress = "8.8.8.8",
                CountryCode = "US",
                Timestamp = DateTime.UtcNow,
                IsBlocked = true,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
            };
            
            // Add the sample attempt
            await _blockedAttemptsRepository.AddBlockedAttemptAsync(sampleAttempt);
            
            // Get the attempts again
            attempts = await _blockedAttemptsRepository.GetBlockedAttemptsAsync(pageIndex, pageSize);
        }
        
        return Ok(attempts);
    }
}