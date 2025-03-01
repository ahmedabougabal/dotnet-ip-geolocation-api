using System.ComponentModel.DataAnnotations;

namespace CountryBlockingAPI.Models;

public class TemporalBlockRequest
{
    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [Range(1, 1440)]
    public int DurationMinutes { get; set; }
}
