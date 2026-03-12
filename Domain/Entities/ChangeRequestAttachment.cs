namespace Domain.Entities;

public class ChangeRequestAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChangeRequestId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public ChangeRequest? ChangeRequest { get; set; }
}
