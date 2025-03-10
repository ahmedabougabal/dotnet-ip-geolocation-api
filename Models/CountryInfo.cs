namespace CountryBlockingAPI.Models;

public class CountryInfo
{
    // error handling 
    public bool Error { get; set; }
    public string Reason { get; set; } = string.Empty;

    // api's json output props from the ipapi.co documentation 
    public string Ip { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? RegionCode { get; set; }
    public string? Country { get; set; }
    public string? CountryCode { get; set; }
    public string? CountryCodeIso3 { get; set; }
    public string? CountryName { get; set; }
    public string? CountryCapital { get; set; }
    public string? CountryTld { get; set; }
    public long? CountryArea { get; set; }
    public long? CountryPopulation { get; set; }
    public string? ContinentCode { get; set; }
    public bool? InEu { get; set; }
    public string? Postal { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Timezone { get; set; }
    public string? UtcOffset { get; set; }
    public string? CountryCallingCode { get; set; }
    public string? Currency { get; set; }
    public string? CurrencyName { get; set; }
    public string? Languages { get; set; }
    public string? Asn { get; set; }
    public string? Org { get; set; }

    public CountryInfo Clone()
    {
        return new CountryInfo
        {
            Error = Error,
            Reason = Reason,
            Ip = Ip,
            City = City,
            Region = Region,
            RegionCode = RegionCode,
            Country = Country,
            CountryCode = CountryCode,
            CountryCodeIso3 = CountryCodeIso3,
            CountryName = CountryName,
            CountryCapital = CountryCapital,
            CountryTld = CountryTld,
            CountryArea = CountryArea,
            CountryPopulation = CountryPopulation,
            ContinentCode = ContinentCode,
            InEu = InEu,
            Postal = Postal,
            Latitude = Latitude,
            Longitude = Longitude,
            Timezone = Timezone,
            UtcOffset = UtcOffset,
            CountryCallingCode = CountryCallingCode,
            Currency = Currency,
            CurrencyName = CurrencyName,
            Languages = Languages,
            Asn = Asn,
            Org = Org
        };
    }
}