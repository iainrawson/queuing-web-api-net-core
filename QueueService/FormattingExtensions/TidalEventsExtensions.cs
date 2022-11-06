using DataSources.TidalData;
using QueueService.Models;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;
using System.Collections.Generic;

namespace QueueService.FormattingExtensions;

using TidalEvents = List<TidalEvent>;

public static class TidalEventsExtensions {
    public static QueueItem ToQueueItem(this TidalEvents tidalEvents) {
        var fromDate = tidalEvents.Min(t => t.Date);

        var e = new EPSON();

        var b = new ByteArrayBuilder();
        b.Append(e.CenterAlign());
        b.Append(e.PrintLine($"TIDE TABLES: {fromDate}"));
        b.Append(e.PrintLine(null));
        b.Append(e.PrintLine(null));
        var rawContent = b.ToArray();

        QueueItem? queueItem = new (Guid.NewGuid(), rawContent);
        return queueItem;
    }
}