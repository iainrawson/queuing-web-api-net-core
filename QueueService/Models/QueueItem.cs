namespace QueueService.Models;
public class QueueItem {
    public QueueItem(Guid name) {
        Name = name;
    }
    public Guid Name { get; set; }
}