using System.ComponentModel.DataAnnotations;

namespace change_committee.ViewModels.Committee;

public class CommitteeMemberFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Department { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
