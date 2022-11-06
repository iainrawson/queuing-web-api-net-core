using QueueService.Models;
using ESCPOS_NET.Emitters;
using ESCPOS_NET.Utilities;

namespace QueueService.FormattingExtensions;

public static class StringExtensions {
    public static QueueItem ToQueueItem(this String text) {
        
        string[] lines = text.Split(
            new string[] { "\r\n", "\r", "\n" },
            StringSplitOptions.None
        );

        var e = new EPSON();

        var b = new ByteArrayBuilder();
        foreach (var line in lines) {
            b.Append(e.PrintLine(line));
        }
        var rawContent = b.ToArray();

        QueueItem? queueItem = new (Guid.NewGuid(), rawContent);
        return queueItem;
    }
}