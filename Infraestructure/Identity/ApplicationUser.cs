using Microsoft.AspNetCore.Identity;

namespace Infraestructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
}
