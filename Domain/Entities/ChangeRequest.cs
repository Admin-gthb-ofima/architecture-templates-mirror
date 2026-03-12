namespace Domain.Entities;

public class ChangeRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TicketNumber { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public Guid ApplicantId { get; set; }
    public string RequestType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDIENTE"; // PENDIENTE, APROBADO, RECHAZADO, EN_PROGRESO, CERRADO
    public string Priority { get; set; } = "MEDIA";   // BAJA, MEDIA, ALTA
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public Applicant? Applicant { get; set; }
    public Project? Project { get; set; }
    public ICollection<ChangeRequestStage> Stages { get; set; } = new List<ChangeRequestStage>();
    public ICollection<ChangeRequestAttachment> Attachments { get; set; } = new List<ChangeRequestAttachment>();
}
