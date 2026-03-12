namespace change_committee.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalRequests { get; set; }
    public int PendingApproval { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public List<RequestRowViewModel> RecentRequests { get; set; } = [];
}

public class RequestRowViewModel
{
    public Guid Id { get; set; }
    public string CreatedDate { get; set; } = string.Empty;
    public string ApplicantName { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TicketNumber { get; set; } = string.Empty;
}
