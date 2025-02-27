using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Interfaces;


public interface IBlockedAttemptsRepository
{
    Task AddBlockedAttemptAsync(BlockedAttempt attempt);
    Task<PaginatedList<BlockedAttempt>> GetBlockedAttemptsAsync(int pageIndex, int pageSize);
}
