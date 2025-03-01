using System.Collections.Concurrent;
using CountryBlockingAPI.Interfaces;
using CountryBlockingAPI.Models;

namespace CountryBlockingAPI.Repositories;

public class BlockedCountryRepository : IBlockedCountryRepository
{
    // country codes as keys and country info as values in this hashtable 
    private readonly ConcurrentDictionary<string, CountryInfo> _blockedCountries = new(StringComparer.OrdinalIgnoreCase); // ignores casing 'us' = "US"


    public Task<bool> AddBlockedCountryAsync(string countryCode , CountryInfo countryInfo)
    {
        if (string.IsNullOrEmpty(countryCode)) // error handling when country code is null or " "
            return Task.FromResult(false);

        // Ensure the country code in the info matches the key
        countryInfo.CountryCode = countryCode.ToUpperInvariant();
        
        return Task.FromResult(_blockedCountries.TryAdd(countryCode.ToUpperInvariant(), countryInfo)); // task for async
    }

    public Task<bool> RemoveBlockedCountryAsync(string countryCode)
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return Task.FromResult(false);

        return Task.FromResult(_blockedCountries.TryRemove(countryCode.ToUpperInvariant(), out _));
    }


    public Task<bool> IsCountryBlockedAsync(string countryCode) // checks if the country exists in the dict
    {
        if (string.IsNullOrWhiteSpace(countryCode))
            return Task.FromResult(false);

        return Task.FromResult(_blockedCountries.ContainsKey(countryCode.ToUpperInvariant())); // returns true if the country is blocked
    }


    public Task<PaginatedList<CountryInfo>> GetBlockedCountriesAsync(int pageIndex, int pageSize, string? searchTerm = null)
    {
        var query = _blockedCountries.Values.AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            query = query.Where(c =>
            (c.CountryCode != null && c.CountryCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
            (c.CountryName != null && c.CountryName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
        }

        // Order by country code for consistent results
        query = query.OrderBy(c => c.CountryCode);

        return Task.FromResult(PaginatedList<CountryInfo>.Create(query, pageIndex, pageSize));
    }

    public Task<IEnumerable<string>> GetAllBlockedCountryCodesAsync()
    {
        return Task.FromResult<IEnumerable<string>>(_blockedCountries.Keys);
    }
}
