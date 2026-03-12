namespace Domain.Entities;

public class ChangeRequestStage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChangeRequestId { get; set; }
    public int Sequence { get; set; }
    public string StageName { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDIENTE"; // COMPLETADO, EN_PROGRESO, PENDIENTE
    public string? Note { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ChangeRequest? ChangeRequest { get; set; }
}
