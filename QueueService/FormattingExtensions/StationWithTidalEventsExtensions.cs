using DataSources.TidalData;
using QueueService.Models;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Collections.Generic;

namespace QueueService.FormattingExtensions;

using TidalEvents = List<TidalEvent>;

public static class StationWithTidalEventsExtensions {
    public static QueueItem ToQueueItem(this StationWithTidalEvents stationWithTidalEvents) {

        var tidalEvents = stationWithTidalEvents.TidalEvents;

        var fromDate = tidalEvents.Min(t => t.Date);

        var e = new EPSON();

        var b = new ByteArrayBuilder();
        b.Append(e.CenterAlign());
        b.Append(e.PrintLine($"Tides for {stationWithTidalEvents.Station.Properties.Name}"));
        b.Append(e.PrintLine(null));

        var dateEvents = tidalEvents.GroupBy(t => t.Date);

        foreach (var dateEvent in dateEvents) {
            b.Append(e.PrintLine($"{dateEvent.Key.ToStringWithOrdinal()}"));
            b.Append(e.PrintLine("----------------------------------------------------------------"));
            foreach (var dateTide in dateEvent) {
                b.Append(e.PrintLine($"{dateTide.EventType} - {dateTide.DateTime:HH:mm} "));
            } 
            b.Append(e.PrintLine(null));
        }

        b.Append(e.PrintLine(null));
        var rawContent = b.ToArray();

        QueueItem? queueItem = new (Guid.NewGuid(), rawContent);
        return queueItem;
    }
}