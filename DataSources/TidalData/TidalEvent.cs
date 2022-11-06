namespace DataSources.TidalData;
public record TidalEvent {
    public EventType EventType { get; set; }
    public DateTime DateTime { get; set; }
    public bool IsApproximateTime { get; set; }
    public Double Height { get; set; }
    public bool IsApproximateHeight { get; set; }
    public bool Filtered { get; set; }
    // TODO: Work out how to deserialize 2022-01-01T00:00:00 as a DateOnly
    public DateTime Date { get; set; }
}

public enum EventType {
    HighWater,
    LowWater
}