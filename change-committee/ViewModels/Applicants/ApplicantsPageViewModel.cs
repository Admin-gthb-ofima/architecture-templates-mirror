namespace change_committee.ViewModels.Applicants;

public class ApplicantsPageViewModel
{
    public string Filter { get; set; } = "todos";
    public string Search { get; set; } = string.Empty;
    public int Total { get; set; }
    public int Active { get; set; }
    public int NewThisMonth { get; set; }
    public int PendingRequests { get; set; }
    public List<ApplicantRowViewModel> Applicants { get; set; } = [];
}

public class ApplicantRowViewModel
{
    public Guid Id { get; set; }
    public string Initials { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
