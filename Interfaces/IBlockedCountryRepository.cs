using CountryBlockingAPI.Models; 

namespace CountryBlockingAPI.Interfaces;

public interface IBlockedCountryRepository
{
    Task<bool> AddBlockedCountryAsync(string countryCode, CountryInfo countryInfo);
    Task<bool> RemoveBlockedCountryAsync(string countryCode);
    Task<bool> IsCountryBlockedAsync(string countryCode);
    Task<PaginatedList<CountryInfo>> GetBlockedCountriesAsync(int pageIndex, int pageSize, string? searchTerm = null );
    Task<IEnumerable<string>> GetAllBlockedCountryCodesAsync();
}
