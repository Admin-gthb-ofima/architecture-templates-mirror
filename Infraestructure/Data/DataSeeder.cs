using Domain.Entities;
using Infraestructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text;

namespace Infraestructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await EnsureRolesAsync(roleManager);
        await EnsureAdminUserAsync(userManager);
        await SeedMasterDataAsync(context);
        await SeedChangeRequestsAsync(context);
    }

    private static async Task EnsureRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Administrador", "Comite", "Solicitante"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task EnsureAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        const string email = "dmartinez@ofima.com";
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Daniel Martinez",
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, "123456789");
            if (!result.Succeeded)
            {
                return;
            }
        }

        string[] roles = ["Administrador", "Comite", "Solicitante"];
        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }

    private static async Task SeedMasterDataAsync(AppDbContext context)
    {
        var existingProjectNames = await context.Projects.Select(x => x.Name).ToListAsync();
        var existingNormalizedProjects = existingProjectNames
            .Select(NormalizeText)
            .ToHashSet();
        var requiredProjects = new List<Project>
        {
            new() { Name = "Migracion Cloud AWS", Description = "Modernizacion de infraestructura critica" },
            new() { Name = "Optimizacion Operativa", Description = "Automatizacion de procesos internos" },
            new() { Name = "Expansion Regional Latam", Description = "Crecimiento comercial por paises" },
            new() { Name = "Portal de Autoservicio HR", Description = "Gestion digital para RRHH" }
        };
        var missingProjects = requiredProjects
            .Where(x => !existingNormalizedProjects.Contains(NormalizeText(x.Name)))
            .ToList();
        if (missingProjects.Count > 0)
        {
            await context.Projects.AddRangeAsync(missingProjects);
        }

        var existingApplicantEmails = await context.Applicants.Select(x => x.Email).ToListAsync();
        var requiredApplicants = new List<Applicant>
        {
            new() { FirstName = "Juan", LastName = "Perez", Email = "juan.perez@empresa.com", Department = "Sistemas", Position = "Analista de TI", Project = "Migracion Cloud AWS", IsActive = true },
            new() { FirstName = "Maria", LastName = "Garcia", Email = "maria.garcia@empresa.com", Department = "Recursos Humanos", Position = "Gerente de RRHH", Project = "Cultura 2024", IsActive = true },
            new() { FirstName = "Carlos", LastName = "Ruiz", Email = "carlos.ruiz@empresa.com", Department = "Infraestructura", Position = "Ingeniero Civil", Project = "Planta Norte", IsActive = false },
            new() { FirstName = "Ana", LastName = "Lopez", Email = "ana.lopez@empresa.com", Department = "Operaciones", Position = "Coordinadora", Project = "Optimizacion Operativa", IsActive = true },
            new() { FirstName = "Lucia", LastName = "Ferrera", Email = "lucia.ferrera@empresa.com", Department = "PMO", Position = "Project Manager", Project = "Migracion Cloud AWS", IsActive = true },
            new() { FirstName = "Roberto", LastName = "Gomez", Email = "roberto.gomez@empresa.com", Department = "Operaciones", Position = "Director Operaciones", Project = "Rediseno Logistico", IsActive = true }
        };
        var missingApplicants = requiredApplicants
            .Where(x => !existingApplicantEmails.Contains(x.Email))
            .ToList();
        if (missingApplicants.Count > 0)
        {
            await context.Applicants.AddRangeAsync(missingApplicants);
        }

        var existingMemberEmails = await context.CommitteeMembers.Select(x => x.Email).ToListAsync();
        var requiredMembers = new List<CommitteeMember>
        {
            new() { Name = "Juan Perez", Role = "Presidente del Comite", Department = "Operaciones", Email = "juan.perez@empresa.com", IsActive = true },
            new() { Name = "Maria Garcia", Role = "Secretaria Tecnica", Department = "Tecnologia", Email = "maria.garcia@empresa.com", IsActive = true },
            new() { Name = "Carlos Ruiz", Role = "Vocal de Seguridad", Department = "Riesgos", Email = "carlos.ruiz@empresa.com", IsActive = true },
            new() { Name = "Ana Lopez", Role = "Consultora de Cumplimiento", Department = "Legal", Email = "ana.lopez@empresa.com", IsActive = true },
            new() { Name = "Sofia Rojas", Role = "Arquitecta Empresarial", Department = "Tecnologia", Email = "sofia.rojas@empresa.com", IsActive = true },
            new() { Name = "Miguel Torres", Role = "Analista de Riesgo", Department = "Riesgos", Email = "miguel.torres@empresa.com", IsActive = false }
        };
        var missingMembers = requiredMembers
            .Where(x => !existingMemberEmails.Contains(x.Email))
            .ToList();
        if (missingMembers.Count > 0)
        {
            await context.CommitteeMembers.AddRangeAsync(missingMembers);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedChangeRequestsAsync(AppDbContext context)
    {
        if (await context.ChangeRequests.AnyAsync())
        {
            return;
        }

        var projects = await context.Projects.OrderBy(x => x.Name).ToListAsync();
        var applicants = await context.Applicants.OrderBy(x => x.FirstName).ToListAsync();
        if (projects.Count < 2 || applicants.Count < 2)
        {
            return;
        }

        var createdAt = DateTime.UtcNow.AddDays(-15);

        var req1 = new ChangeRequest
        {
            TicketNumber = "CR-2023-8901",
            ProjectId = projects[0].Id,
            ApplicantId = applicants[3].Id,
            RequestType = "Infraestructura Critica",
            Description = "Actualizacion de parches de seguridad para servidores de produccion.",
            Status = "EN_PROGRESO",
            Priority = "ALTA",
            CreatedAt = createdAt,
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

        var req2 = new ChangeRequest
        {
            TicketNumber = "CR-2023-8894",
            ProjectId = projects[1].Id,
            ApplicantId = applicants[4].Id,
            RequestType = "Rediseno Operativo",
            Description = "Ajuste de flujo logistico para optimizar tiempos de entrega.",
            Status = "APROBADO",
            Priority = "MEDIA",
            CreatedAt = createdAt.AddDays(2),
            UpdatedAt = DateTime.UtcNow.AddHours(-4)
        };

        var req3 = new ChangeRequest
        {
            TicketNumber = "CR-2023-8887",
            ProjectId = projects[2].Id,
            ApplicantId = applicants[5].Id,
            RequestType = "Expansion Plataforma",
            Description = "Solicitud para despliegue regional en Chile y Peru.",
            Status = "RECHAZADO",
            Priority = "MEDIA",
            CreatedAt = createdAt.AddDays(4),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var req4 = new ChangeRequest
        {
            TicketNumber = "CR-2023-8879",
            ProjectId = projects[3].Id,
            ApplicantId = applicants[0].Id,
            RequestType = "Portal RH",
            Description = "Nueva funcionalidad para autoservicio de vacaciones.",
            Status = "PENDIENTE",
            Priority = "BAJA",
            CreatedAt = createdAt.AddDays(6),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };

        await context.ChangeRequests.AddRangeAsync(req1, req2, req3, req4);

        await context.ChangeRequestStages.AddRangeAsync(
            BuildStage(req1.Id, 1, "Solicitud Recibida", "COMPLETADO", "Radicado registrado", req1.CreatedAt),
            BuildStage(req1.Id, 2, "Revision Tecnica", "EN_PROGRESO", "Analista revisando dependencias", req1.UpdatedAt),
            BuildStage(req1.Id, 3, "Aprobacion Comite", "PENDIENTE", null, null),
            BuildStage(req1.Id, 4, "Implementacion", "PENDIENTE", null, null),
            BuildStage(req1.Id, 5, "Cerrado", "PENDIENTE", null, null),

            BuildStage(req2.Id, 1, "Solicitud Recibida", "COMPLETADO", "Radicado registrado", req2.CreatedAt),
            BuildStage(req2.Id, 2, "Revision Tecnica", "COMPLETADO", "Revision sin hallazgos", req2.CreatedAt.AddDays(1)),
            BuildStage(req2.Id, 3, "Aprobacion Comite", "COMPLETADO", "Aprobacion unanime", req2.CreatedAt.AddDays(2)),
            BuildStage(req2.Id, 4, "Implementacion", "EN_PROGRESO", "Despliegue programado para esta noche", req2.UpdatedAt),
            BuildStage(req2.Id, 5, "Cerrado", "PENDIENTE", null, null),

            BuildStage(req3.Id, 1, "Solicitud Recibida", "COMPLETADO", "Radicado registrado", req3.CreatedAt),
            BuildStage(req3.Id, 2, "Revision Tecnica", "COMPLETADO", "Riesgo alto identificado", req3.CreatedAt.AddDays(1)),
            BuildStage(req3.Id, 3, "Aprobacion Comite", "COMPLETADO", "Comite rechazo por impacto operativo", req3.UpdatedAt),
            BuildStage(req3.Id, 4, "Implementacion", "PENDIENTE", null, null),
            BuildStage(req3.Id, 5, "Cerrado", "PENDIENTE", null, null),

            BuildStage(req4.Id, 1, "Solicitud Recibida", "COMPLETADO", "Radicado registrado", req4.CreatedAt),
            BuildStage(req4.Id, 2, "Revision Tecnica", "PENDIENTE", null, null),
            BuildStage(req4.Id, 3, "Aprobacion Comite", "PENDIENTE", null, null),
            BuildStage(req4.Id, 4, "Implementacion", "PENDIENTE", null, null),
            BuildStage(req4.Id, 5, "Cerrado", "PENDIENTE", null, null)
        );

        await context.ChangeRequestAttachments.AddRangeAsync(
            new ChangeRequestAttachment { ChangeRequestId = req1.Id, FileName = "plan_implementacion.pdf", ContentType = "application/pdf", SizeInBytes = 1258291 },
            new ChangeRequestAttachment { ChangeRequestId = req1.Id, FileName = "analisis_riesgo.xlsx", ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", SizeInBytes = 460800 },
            new ChangeRequestAttachment { ChangeRequestId = req2.Id, FileName = "roadmap_logistico.docx", ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document", SizeInBytes = 350000 }
        );

        await context.SaveChangesAsync();
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

    private static string NormalizeText(string value)
    {
        var normalized = value.Normalize(NormalizationForm.FormD);
        var buffer = new StringBuilder(value.Length);
        foreach (var c in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            {
                buffer.Append(char.ToLowerInvariant(c));
            }
        }

        return buffer.ToString().Normalize(NormalizationForm.FormC).Trim();
    }
}
