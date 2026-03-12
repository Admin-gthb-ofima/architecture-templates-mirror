using System.ComponentModel.DataAnnotations;

namespace change_committee.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Correo invalido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contrasena es obligatoria.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}
