using CountryBlockingAPI.Models; 

namespace CountryBlockingAPI.Interfaces;

public interface IGeolocationService
{
    Task<CountryInfo?> GetCountryInfoByIpAsync(string ipAddress); // the use of task --> async operation for the HTTP CALLS to the external ipapi.co
}



