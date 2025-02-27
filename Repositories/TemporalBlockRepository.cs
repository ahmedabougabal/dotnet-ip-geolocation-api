using System.Collections.Concurrent
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Repositories;

public class TemporalBlockRepository : ITemporalBlockRepository
{
    private readonly ConcurrentDictionary<string, TemporalBlock> _temporalBlocks = new(stringComparer.OrdinalIgnoreCase);

    public Task<bool> AddTemporalBlockSync(TemporalBlock temporalBlock)
    {
        if (string.IsNullOrWhiteSpace(temporalBlock.CountryCode))
            return Task.FromResult(false);
        
        return Task.FromResult(_temporalBlocks.TryAdd(temporalBlock.CountryCode, temporalBlock));
    }


}







