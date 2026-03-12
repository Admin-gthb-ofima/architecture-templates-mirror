namespace change_committee.ViewModels.ChangeRequests;

public class TrackChangeRequestViewModel
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string LastUpdate { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TechnicalNote { get; set; } = string.Empty;
    public List<StageViewModel> Stages { get; set; } = [];
    public List<AttachmentViewModel> Attachments { get; set; } = [];
}

public class StageViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string DateLabel { get; set; } = string.Empty;
}

public class AttachmentViewModel
{
    public string FileName { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
}
