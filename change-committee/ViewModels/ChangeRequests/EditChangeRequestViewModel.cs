using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace change_committee.ViewModels.ChangeRequests;

public class EditChangeRequestViewModel
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid ApplicantId { get; set; }

    [Required]
    [StringLength(100)]
    public string RequestType { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 20)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Priority { get; set; } = "MEDIA";

    [Required]
    public string Status { get; set; } = "PENDIENTE";

    public List<SelectListItem> Projects { get; set; } = [];
    public List<SelectListItem> Applicants { get; set; } = [];
}
