namespace change_committee.ViewModels.Projects;

public class ProjectsPageViewModel
{
    public int TotalProjects { get; set; }
    public int ActiveRequests { get; set; }
    public List<ProjectRowViewModel> Projects { get; set; } = [];
}

public class ProjectRowViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
}
