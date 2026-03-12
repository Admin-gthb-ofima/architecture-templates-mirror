using change_committee.ViewModels.Committee;
using Domain.Entities;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace change_committee.Controllers;

[Authorize]
public class CommitteeController : Controller
{
    private readonly AppDbContext _db;

    public CommitteeController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var members = await _db.CommitteeMembers
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CommitteeMemberRowViewModel
            {
                Id = x.Id,
                Initials = BuildInitials(x.Name),
                Name = x.Name,
                Role = x.Role,
                Department = x.Department,
                Email = x.Email,
                IsActive = x.IsActive
            })
            .ToListAsync();

        var model = new CommitteePageViewModel
        {
            TotalMembers = members.Count,
            ActiveMembers = members.Count(x => x.IsActive),
            PendingMembers = members.Count(x => !x.IsActive),
            Members = members
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CommitteeMemberFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CommitteeMemberFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var emailExists = await _db.CommitteeMembers.AnyAsync(x => x.Email == model.Email);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Ya existe un integrante con ese correo.");
            return View(model);
        }

        _db.CommitteeMembers.Add(new CommitteeMember
        {
            Name = model.Name,
            Role = model.Role,
            Department = model.Department,
            Email = model.Email,
            IsActive = model.IsActive
        });

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Integrante creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _db.CommitteeMembers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        return View(new CommitteeMemberFormViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Role = entity.Role,
            Department = entity.Department,
            Email = entity.Email,
            IsActive = entity.IsActive
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CommitteeMemberFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = await _db.CommitteeMembers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            return NotFound();
        }

        var emailExists = await _db.CommitteeMembers.AnyAsync(x => x.Email == model.Email && x.Id != id);
        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email), "Ya existe un integrante con ese correo.");
            return View(model);
        }

        entity.Name = model.Name;
        entity.Role = model.Role;
        entity.Department = model.Department;
        entity.Email = model.Email;
        entity.IsActive = model.IsActive;

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Integrante actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.CommitteeMembers.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            TempData["SuccessMessage"] = "El integrante ya no existe.";
            return RedirectToAction(nameof(Index));
        }

        _db.CommitteeMembers.Remove(entity);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = "Integrante eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private static string BuildInitials(string fullName)
    {
        var parts = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Take(2)
            .Select(x => x[0].ToString().ToUpperInvariant())
            .ToList();

        return parts.Count == 0 ? "CM" : string.Concat(parts);
    }
}
