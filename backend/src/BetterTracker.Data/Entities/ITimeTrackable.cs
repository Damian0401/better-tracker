namespace BetterTracker.Data.Entities;

public interface ITimeTrackable
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
