namespace QueueService.Models;
public class QueueItem {
    public QueueItem(Guid id) {
        Id = id;
    }
    public Guid Id { get; set; }
}