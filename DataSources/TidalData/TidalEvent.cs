namespace DataSources.TidalData;

using TidalEvents = List<TidalEvent>;

public record StationWithTidalEvents {
    public Station Station { get; set; }
    public TidalEvents TidalEvents { get; set; }
}
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

public record Station {
    public LocationType Type { get; set; }
    public Geometry Geometry { get; set; }
    public string Country { get; set; }
    public Properties Properties { get; set; }
}

public enum LocationType {
    Point,
    MultiPoint,
    LineString,
    MultiLineString,
    Polygon,
    MultiPolygon,
    GeometryCollection,
    Feature,
    FeatureCollection
}

public record Geometry {
    public LocationType Type { get; set; }
    public List<Decimal> Coordinates { get; set; }
}

public record Properties {
    public string Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public bool ContinuousHeightsAvailable { get; set; }
    public string Footnote { get; set; }
}

