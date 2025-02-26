namespace CountryBlockingAPI.Models;

public class BlockedAttempt
{
    public string IpAddress { get; set; } = string.Empty; // ensures the properties are never null as a result of using
    //an in-memory storage as required in the assignment 
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow; // the same cause why we have used '.Utc' here as to prevent null reference exceptions
    public string CountryCode { get; set; } = string.Empty;
    public bool IsBlocked { get; set; }
    public string UserAgent { get; set; } = string.Empty;
}