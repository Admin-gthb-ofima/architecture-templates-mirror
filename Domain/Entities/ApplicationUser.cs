namespace Domain.Entities;

// Perfil de usuario propio del dominio que referencia al Id del usuario de Identity
public class AppUserProfile
{
    public string UserId { get; set; } = string.Empty; // Id del usuario en Identity
    public string? FullName { get; set; }
    public bool IsActive { get; set; } = true;
}
