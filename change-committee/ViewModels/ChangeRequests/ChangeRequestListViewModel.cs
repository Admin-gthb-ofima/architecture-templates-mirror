namespace change_committee.ViewModels.ChangeRequests;

public class ChangeRequestListViewModel
{
    public int Total { get; set; }
    public List<ChangeRequestListRowViewModel> Items { get; set; } = [];
}

public class ChangeRequestListRowViewModel
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
