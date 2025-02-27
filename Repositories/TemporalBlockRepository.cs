using System.Collections.Concurrent;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Repositories;

public class TemporalBlockRepository : ITemporalBlockRepository
{
    private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks = new(StringComparer.OrdinalIgnoreCase);

    public Task<bool> AddTemporalBlockAsync(TemporalBlock temporalBlock)
    {
        if (string.IsNullOrWhiteSpace(temporalBlock.CountryCode))
            return Task.FromResult(false);
        
        return Task.FromResult(_temporalBlocks.TryAdd(temporalBlock.CountryCode, temporalBlock));
    }

    public Task<bool> RemoveTemporalBlockAsync(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return Task.FromResult(false);

        return Task.FromResult(_temporalBlocks.TryRemove(countryCode, out _));
    }

    public Task<bool> IsCountryTemporallyBlockedAsync(string countryCode) // check if a country is temp blocked
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return Task.FromResult(false);

        // checks if country exists
        if (!_temporalBlocks.TryGetValue(countryCode, out var block)) 
            return Task.FromResult(false);

        //this is condition is for checking whether the block on a country has expired or not
        if (block.ExpirationTime < DateTime.UtcNow)
        {
            _temporalBlocks.TryRemove(countryCode, out _); // remove expired block of a country
            return Task.FromResult(false); // country is no longer blocked
        }

        return Task.FromResult(true); // country is blocked
    }


    public Task<IEnumerable<TemporalBlock>> GetExpiredTemporalBlocksAsync()
    {
        var now = DateTime.UtcNow;
        var expiredBlocks = _temporalBlocks.Values
            .Where(block => block.ExpirationTime < now)
            .ToList();

        return Task.FromResult<IEnumerable<TemporalBlock>>(expiredBlocks);
    }


    public Task<IEnumerable<string>> GetAllTemporallyBlockedCountryCodesAsync()
    {
        var now = DateTime.UtcNow;
        var validBlocks = _temporalBlocks.Values
            .Where(block => block.ExpirationTime >= now)
            .Select(block => block.CountryCode)
            .ToList();

        return Task.FromResult<IEnumerable<string>>(validBlocks);
    }
}
