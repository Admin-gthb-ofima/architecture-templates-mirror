using change_committee.ViewModels.Dashboard;
using Infraestructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace change_committee.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var rows = await _db.ChangeRequests
            .AsNoTracking()
            .Include(x => x.Applicant)
            .Include(x => x.Project)
            .OrderByDescending(x => x.CreatedAt)
            .Take(8)
            .Select(x => new RequestRowViewModel
            {
                Id = x.Id,
                CreatedDate = x.CreatedAt.ToString("dd/MM/yyyy"),
                ApplicantName = x.Applicant != null ? $"{x.Applicant.FirstName} {x.Applicant.LastName}" : "Sin solicitante",
                ProjectName = x.Project != null ? x.Project.Name : "Sin proyecto",
                Position = x.Applicant != null ? x.Applicant.Position : "Sin cargo",
                Status = x.Status,
                TicketNumber = x.TicketNumber
            })
            .ToListAsync();

        var model = new DashboardViewModel
        {
            TotalRequests = await _db.ChangeRequests.CountAsync(),
            PendingApproval = await _db.ChangeRequests.CountAsync(x => x.Status == "PENDIENTE" || x.Status == "EN_PROGRESO"),
            Approved = await _db.ChangeRequests.CountAsync(x => x.Status == "APROBADO"),
            Rejected = await _db.ChangeRequests.CountAsync(x => x.Status == "RECHAZADO"),
            RecentRequests = rows
        };

        return View(model);
    }
}
