using change_committee.ViewModels.Projects;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace change_committee.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var projects = await _db.Projects
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ProjectRowViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description ?? "Sin descripcion",
                TotalRequests = x.ChangeRequests.Count,
                PendingRequests = x.ChangeRequests.Count(r => r.Status == "PENDIENTE" || r.Status == "EN_PROGRESO")
            })
            .ToListAsync();

        var model = new ProjectsPageViewModel
        {
            TotalProjects = projects.Count,
            ActiveRequests = projects.Sum(x => x.PendingRequests),
            Projects = projects
        };

        return View(model);
    }
}
