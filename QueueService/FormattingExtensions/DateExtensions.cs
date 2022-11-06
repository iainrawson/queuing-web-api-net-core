namespace QueueService.FormattingExtensions;

public static class DateExtensions {
        public static string ToStringWithOrdinal(this DateTime d) {
        switch (d.Day) {
            case 1: case 21: case 31:
            return d.ToString("dddd d'st' MMMM yyyy");
            case 2: case 22:
            return d.ToString("dddd d'nd' MMMM yyyy");
            case 3: case 23:
            return d.ToString("dddd d'rd' MMMM yyyy");
            default:
            return d.ToString("dddd d'th' MMMM yyyy");
        };
    }
}