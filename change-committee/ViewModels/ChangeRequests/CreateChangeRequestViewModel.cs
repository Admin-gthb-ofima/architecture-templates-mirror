using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace change_committee.ViewModels.ChangeRequests;

public class CreateChangeRequestViewModel
{
    [Required(ErrorMessage = "Seleccione un proyecto.")]
    public Guid? ProjectId { get; set; }

    [Required(ErrorMessage = "Seleccione un solicitante.")]
    public Guid? ApplicantId { get; set; }

    [Required(ErrorMessage = "Seleccione el tipo de solicitud.")]
    public string RequestType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Describa la solicitud.")]
    [StringLength(2000, MinimumLength = 20, ErrorMessage = "La descripcion debe tener al menos 20 caracteres.")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Seleccione una prioridad.")]
    public string Priority { get; set; } = "MEDIA";

    public List<IFormFile> EvidenceFiles { get; set; } = [];
    public List<SelectListItem> Projects { get; set; } = [];
    public List<SelectListItem> Applicants { get; set; } = [];
}
