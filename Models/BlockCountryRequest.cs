using System.ComponentModel.DataAnnotations;

namespace CountryBlockingAPI.Models;

public class BlockCountryRequest
{
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;
}
