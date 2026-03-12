namespace change_committee.ViewModels.Committee;

public class CommitteePageViewModel
{
    public int TotalMembers { get; set; }
    public int ActiveMembers { get; set; }
    public int PendingMembers { get; set; }
    public List<CommitteeMemberRowViewModel> Members { get; set; } = [];
}

public class CommitteeMemberRowViewModel
{
    public Guid Id { get; set; }
    public string Initials { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
