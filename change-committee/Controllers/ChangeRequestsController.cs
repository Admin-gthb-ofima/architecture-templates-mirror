using change_committee.ViewModels.ChangeRequests;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace change_committee.Controllers;

[Authorize]
public class ChangeRequestsController : Controller
{
    private readonly AppDbContext _db;

    public ChangeRequestsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await _db.ChangeRequests
            .AsNoTracking()
            .Include(x => x.Applicant)
            .Include(x => x.Project)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ChangeRequestListRowViewModel
            {
                Id = x.Id,
                TicketNumber = x.TicketNumber,
                ProjectName = x.Project != null ? x.Project.Name : "Sin proyecto",
                ApplicantName = x.Applicant != null ? x.Applicant.FirstName + " " + x.Applicant.LastName : "Sin solicitante",
                RequestType = x.RequestType,
                Priority = x.Priority,
                Status = x.Status,
                CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy")
            })
            .ToListAsync();

        return View(new ChangeRequestListViewModel
        {
            Total = items.Count,
            Items = items
        });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateChangeRequestViewModel();
        await PopulateCreateOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateChangeRequestViewModel model)
    {
        if (!ModelState.IsValid || model.ProjectId is null || model.ApplicantId is null)
        {
            await PopulateCreateOptionsAsync(model);
            return View(model);
        }

        var now = DateTime.UtcNow;
        var ticket = await GenerateTicketNumberAsync(now.Year);
        var entity = new ChangeRequest
        {
            TicketNumber = ticket,
            ProjectId = model.ProjectId.Value,
            ApplicantId = model.ApplicantId.Value,
            RequestType = model.RequestType,
            Description = model.Description,
            Priority = model.Priority,
            Status = "PENDIENTE",
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.ChangeRequests.Add(entity);
        _db.ChangeRequestStages.AddRange(
            BuildStage(entity.Id, 1, "Solicitud Recibida", "COMPLETADO", "Radicado recibido por el sistema.", now),
            BuildStage(entity.Id, 2, "Revision Tecnica", "PENDIENTE", null, null),
            BuildStage(entity.Id, 3, "Aprobacion Comite", "PENDIENTE", null, null),
            BuildStage(entity.Id, 4, "Implementacion", "PENDIENTE", null, null),
            BuildStage(entity.Id, 5, "Cerrado", "PENDIENTE", null, null)
        );

        foreach (var file in model.EvidenceFiles.Where(x => x.Length > 0))
        {
            _db.ChangeRequestAttachments.Add(new ChangeRequestAttachment
            {
                ChangeRequestId = entity.Id,
                FileName = file.FileName,
                ContentType = file.ContentType ?? "application/octet-stream",
                SizeInBytes = file.Length
            });
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Solicitud registrada con exito. Radicado: {entity.TicketNumber}";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _db.ChangeRequests.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        var model = new EditChangeRequestViewModel
        {
            Id = entity.Id,
            TicketNumber = entity.TicketNumber,
            ProjectId = entity.ProjectId,
            ApplicantId = entity.ApplicantId,
            RequestType = entity.RequestType,
            Description = entity.Description,
            Priority = entity.Priority,
            Status = entity.Status
        };
        await PopulateEditOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditChangeRequestViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateEditOptionsAsync(model);
            return View(model);
        }

        var entity = await _db.ChangeRequests
            .Include(x => x.Stages)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        entity.ProjectId = model.ProjectId;
        entity.ApplicantId = model.ApplicantId;
        entity.RequestType = model.RequestType;
        entity.Description = model.Description;
        entity.Priority = model.Priority;
        entity.Status = model.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        UpdateStagesForStatus(entity.Stages, entity.Status, entity.UpdatedAt.Value);

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.ChangeRequests.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            TempData["SuccessMessage"] = "La solicitud ya no existe.";
            return RedirectToAction(nameof(Index));
        }

        _db.ChangeRequests.Remove(entity);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitud eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Track(Guid? id)
    {
        var query = _db.ChangeRequests
            .AsNoTracking()
            .Include(x => x.Applicant)
            .Include(x => x.Project)
            .Include(x => x.Stages)
            .Include(x => x.Attachments)
            .AsQueryable();

        var entity = id.HasValue
            ? await query.FirstOrDefaultAsync(x => x.Id == id.Value)
            : await query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt).FirstOrDefaultAsync();

        if (entity is null)
        {
            return RedirectToAction(nameof(Index));
        }

        var model = new TrackChangeRequestViewModel
        {
            Id = entity.Id,
            TicketNumber = entity.TicketNumber,
            LastUpdate = BuildRelativeTime(entity.UpdatedAt ?? entity.CreatedAt),
            RequestType = entity.RequestType,
            ApplicantName = entity.Applicant is null ? "Sin solicitante" : $"{entity.Applicant.FirstName} {entity.Applicant.LastName}",
            Department = entity.Applicant?.Department ?? "Sin departamento",
            Priority = entity.Priority,
            Description = entity.Description,
            TechnicalNote = entity.Stages
                .OrderBy(x => x.Sequence)
                .FirstOrDefault(x => x.Status == "EN_PROGRESO")?.Note ?? "El caso esta pendiente de asignacion tecnica.",
            Stages = entity.Stages
                .OrderBy(x => x.Sequence)
                .Select(x => new StageViewModel
                {
                    Name = x.StageName,
                    Status = x.Status,
                    DateLabel = x.UpdatedAt?.ToString("dd MMM, yyyy") ?? "Pendiente"
                })
                .ToList(),
            Attachments = entity.Attachments
                .OrderByDescending(x => x.UploadedAt)
                .Select(x => new AttachmentViewModel
                {
                    FileName = x.FileName,
                    SizeLabel = BuildSizeLabel(x.SizeInBytes)
                })
                .ToList()
        };

        return View(model);
    }

    private async Task PopulateCreateOptionsAsync(CreateChangeRequestViewModel model)
    {
        model.Projects = await _db.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();

        model.Applicants = await _db.Applicants
            .AsNoTracking()
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName + " " + x.LastName })
            .ToListAsync();
    }

    private async Task PopulateEditOptionsAsync(EditChangeRequestViewModel model)
    {
        model.Projects = await _db.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
            .ToListAsync();

        model.Applicants = await _db.Applicants
            .AsNoTracking()
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.FirstName + " " + x.LastName })
            .ToListAsync();
    }

    private async Task<string> GenerateTicketNumberAsync(int year)
    {
        var sequence = await _db.ChangeRequests.CountAsync() + 8901;
        return $"CR-{year}-{sequence:D4}";
    }

    private static ChangeRequestStage BuildStage(
        Guid requestId,
        int sequence,
        string stageName,
        string status,
        string? note,
        DateTime? updatedAt)
    {
        return new ChangeRequestStage
        {
            ChangeRequestId = requestId,
            Sequence = sequence,
            StageName = stageName,
            Status = status,
            Note = note,
            UpdatedAt = updatedAt
        };
    }

    private static void UpdateStagesForStatus(IEnumerable<ChangeRequestStage> stages, string status, DateTime updatedAt)
    {
        var ordered = stages.OrderBy(x => x.Sequence).ToList();
        if (ordered.Count == 0)
        {
            return;
        }

        var progressSequence = status switch
        {
            "PENDIENTE" => 2,
            "EN_PROGRESO" => 2,
            "APROBADO" => 4,
            "RECHAZADO" => 3,
            "CERRADO" => 5,
            _ => 2
        };

        foreach (var stage in ordered)
        {
            if (stage.Sequence < progressSequence)
            {
                stage.Status = "COMPLETADO";
                stage.UpdatedAt = updatedAt;
            }
            else if (stage.Sequence == progressSequence && status != "CERRADO")
            {
                stage.Status = "EN_PROGRESO";
                stage.UpdatedAt = updatedAt;
            }
            else if (stage.Sequence == progressSequence && status == "CERRADO")
            {
                stage.Status = "COMPLETADO";
                stage.UpdatedAt = updatedAt;
            }
            else
            {
                stage.Status = "PENDIENTE";
            }
        }
    }

    private static string BuildSizeLabel(long sizeInBytes)
    {
        var kb = sizeInBytes / 1024.0;
        if (kb < 1024)
        {
            return $"{Math.Round(kb)} KB";
        }

        return $"{(kb / 1024.0):0.0} MB";
    }

    private static string BuildRelativeTime(DateTime reference)
    {
        var diff = DateTime.UtcNow - reference;
        if (diff.TotalMinutes < 1) return "Ultima actualizacion: hace segundos";
        if (diff.TotalMinutes < 60) return $"Ultima actualizacion: hace {(int)diff.TotalMinutes} minutos";
        if (diff.TotalHours < 24) return $"Ultima actualizacion: hace {(int)diff.TotalHours} horas";
        return $"Ultima actualizacion: hace {(int)diff.TotalDays} dias";
    }
}
