using System.ComponentModel.DataAnnotations;

namespace change_committee.ViewModels.Applicants;

public class ApplicantFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Position { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Department { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Project { get; set; }

    public bool IsActive { get; set; } = true;
}
