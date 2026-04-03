namespace BetterTracker.Data;

public interface ITimeTrackable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
