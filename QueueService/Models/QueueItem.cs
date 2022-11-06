namespace QueueService.Models;
public class QueueItem {
    public QueueItem(Guid id, Byte[] rawContent) {
        Id = id;
        RawContent = rawContent;
    }

    public Guid Id { get; set; }
    public Byte[] RawContent { get; set; }

}
