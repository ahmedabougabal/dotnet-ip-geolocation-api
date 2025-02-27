using System.Collections.Concurrent;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Repositories;


public class BlockedAttemptsRepository : IBlockedAttemptsRepository // implemeting class for logging blocked attempts
{
    // adds bag to add items in it not a key-value pair, order isnot a priority here
    private readonly ConcurrentBag<BlockedAttempt> _blockedAttempts = new(); // thread-safe collection as always

    public Task AddBlockedAttemptAsync(BlockedAttempt attempt)
    {
        _blockedAttempts.Add(attempt); 

        return Task.CompletedTask;
    }

    public Task<PaginatedList<BlockedAttempt>> GetBlockedAttemptsAsync(int pageIndex,  int pageSize) // params for pagination
    {
        // the logic here orders by most recent attempts first.
        var OrderedAttempts = _blockedAttempts
            .OrderByDescending(attempt => attempt.Timestamp)
            .AsQueryable();

        return Task.FromResult(PaginatedList<BlockedAttempt>.Create(OrderedAttempts, pageIndex, pageSize));
    }

}



