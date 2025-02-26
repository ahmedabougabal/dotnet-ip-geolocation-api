using System.Collections.ConCurrent; // as per task requiement , works safely when multiple users access this dict simaltanously
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Repositories;

public class BlockedCountryRepository : IBlockedCountryRepository
{
    // country codes as keys and country info as values in this hashtable 
    private readonly ConcurrentDictionary<string, CountryInfo> _blockedCountries = new(StringComparer.OrdinalIgnoreCase); // ignores casing 'us' = "US"


    public Task<bool> AddBlockedCountryAsync(string countryCode, CountryInfo countryInfo)
    {
        if (string.IsNullOrEmpty(countryCode)) // error handling when country code is null or " "
            return Task.FromResult(false);
        
        return Task.FromResult(_blockedCountries.TryAdd(countryCode, countryInfo)); // task for async
    }

    public Task<bool> RemoveBlockedCountriesAsync(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode)
            return Task.FromResult(false;)

        return Task.FromResult(_blockedCountries.TryRemove(countryCode, out_));
    }


    public Task<bool> IsCountryBlockedAsync(string countryCode) // checks if the country exists in the dict
    {
        if (string.IsNullorWhiteSpace(countryCode))
            return Task.FromResult(false;)

        return Task.FromResult(_blockedCountries.ContainsKey(countryCode)); // returns true if the country is blocked
    }


    public Task<PaginatedList<CountryInfo>> GetBlockedCountriesAsync(int pageIndex, int pageSize, string? searchTerm = null)
    {
        var query = _blockedCountries.Values.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c =>
            (c.countryCode != null && c.countryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (c.countryName != null && c.countryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        return Task.fromResult(PaginatedList<CountryInfo>.Create(query, pageIndex, pageSize));
    }

    public Task<IEnumerable<string>> GetAllBlockedCountryCodesAsync()
    {
        return Task.FromResult<IEnumerable<string>>(_blockedCountries.Keys);
    }
}



