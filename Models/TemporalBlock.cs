namespace CountryBlockingAPI.Models;

public class TemporalBlock
{
    public string CountryCode { get; set; } = string.Empty; // to store the 2 letter country code that is temp blocked
    public DateTime ExpirationTime { get; set; } // records when the block is expired, the country should be auto removed
    public int DurationMinutes { get; set; } // this is for the background service that should be running every 
    //5 mins to remove expired temp blocks
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}