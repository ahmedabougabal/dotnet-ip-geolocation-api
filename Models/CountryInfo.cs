namespace CountryBlockingAPI.Models;

public class CountryInfo
{
    public string? Ip { get; set; }
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
}