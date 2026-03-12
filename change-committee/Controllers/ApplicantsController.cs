using change_committee.ViewModels.Applicants;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace change_committee.Controllers;

[Authorize]
public class ApplicantsController : Controller
{
    private readonly AppDbContext _db;

    public ApplicantsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? filter, string? q)
    {
        var normalizedFilter = (filter ?? "todos").Trim().ToLowerInvariant();
        var normalizedSearch = (q ?? string.Empty).Trim();

        var query = _db.Applicants.AsNoTracking().AsQueryable();

        if (normalizedFilter == "activos")
        {
            query = query.Where(x => x.IsActive);
        }
        else if (normalizedFilter == "inactivos")
        {
            query = query.Where(x => !x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(x =>
                (x.FirstName + " " + x.LastName).Contains(normalizedSearch) ||
                x.Email.Contains(normalizedSearch) ||
                x.Department.Contains(normalizedSearch));
        }

        var items = await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new ApplicantRowViewModel
            {
                Id = x.Id,
                Initials = (x.FirstName.Substring(0, 1) + x.LastName.Substring(0, 1)).ToUpper(),
                FullName = x.FirstName + " " + x.LastName,
                Position = x.Position,
                Department = x.Department,
                Project = x.Project ?? "No asignado",
                Email = x.Email,
                IsActive = x.IsActive
            })
            .ToListAsync();

        var now = DateTime.UtcNow;
        var model = new ApplicantsPageViewModel
        {
            Filter = normalizedFilter,
            Search = normalizedSearch,
            Total = await _db.Applicants.CountAsync(),
            Active = await _db.Applicants.CountAsync(x => x.IsActive),
            NewThisMonth = await _db.Applicants.CountAsync(x => x.CreatedAt.Month == now.Month && x.CreatedAt.Year == now.Year),
            PendingRequests = await _db.ChangeRequests.CountAsync(x => x.Status == "PENDIENTE" || x.Status == "EN_PROGRESO"),
            Applicants = items
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new ApplicantFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ApplicantFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var emailExists = await _db.Applicants.AnyAsync(x => x.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Ya existe un solicitante con ese correo.");
            return View(model);
        }

        _db.Applicants.Add(new Applicant
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Position = model.Position,
            Department = model.Department,
            Project = model.Project,
            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitante creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _db.Applicants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new ApplicantFormViewModel
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            Position = entity.Position,
            Department = entity.Department,
            Project = entity.Project,
            IsActive = entity.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, ApplicantFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _db.Applicants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        var emailExists = await _db.Applicants.AnyAsync(x => x.Email == model.Email && x.Id != id);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Ya existe un solicitante con ese correo.");
            return View(model);
        }

        entity.FirstName = model.FirstName;
        entity.LastName = model.LastName;
        entity.Email = model.Email;
        entity.Position = model.Position;
        entity.Department = model.Department;
        entity.Project = model.Project;
        entity.IsActive = model.IsActive;

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitante actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Applicants.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            TempData["SuccessMessage"] = "El solicitante ya no existe.";
            return RedirectToAction(nameof(Index));
        }

        var hasRequests = await _db.ChangeRequests.AnyAsync(x => x.ApplicantId == id);
        if (hasRequests)
        {
            entity.IsActive = false;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "El solicitante tenia solicitudes asociadas y fue desactivado.";
            return RedirectToAction(nameof(Index));
        }

        _db.Applicants.Remove(entity);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Solicitante eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
