using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Interfaces;

public interface ITemporalBlockRepository
{
    Task<bool> AddTemporalBlockAsync(TemporalBlock temporalBlock);
    Task<bool> RemoveTemporalCountryAsync(string countryCode);
    Task<bool> IsCountryTemporallyBlockedAsync(string countryCode);
    Task<IEnumerable<TemporalBlock>> GetExpiredTemporalBlocksAsync();
    Task<IEnumerable<string>> GetAllTemporallyBlockedCountryCodesAsync();
    Task<int> RemoveExpiredBlocksAsync();
}
