using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Parcial.Data;
using Parcial.Models;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CursosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? nombre, int? creditosMin, int? creditosMax, TimeSpan? horarioInicio, TimeSpan? horarioFin)
    {
        var cursos = await _context.Cursos.Where(c => c.Activo).ToListAsync();

        if (!string.IsNullOrEmpty(nombre))
            cursos = cursos.Where(c => c.Nombre.Contains(nombre)).ToList();

        if (creditosMin.HasValue)
            cursos = cursos.Where(c => c.Creditos >= creditosMin.Value).ToList();

        if (creditosMax.HasValue)
            cursos = cursos.Where(c => c.Creditos <= creditosMax.Value).ToList();

        if (horarioInicio.HasValue)
            cursos = cursos.Where(c => c.HorarioInicio >= horarioInicio.Value).ToList();

        if (horarioFin.HasValue)
            cursos = cursos.Where(c => c.HorarioFin <= horarioFin.Value).ToList();

        return View(cursos);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null || !curso.Activo)
            return NotFound();

        return View(curso);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Inscribirse(int id)
    {
        var userId = _userManager.GetUserId(User);
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null || !curso.Activo)
            return NotFound();

        var inscritos = await _context.Matriculas
            .CountAsync(m => m.CursoId == id && m.Estado != "Cancelada");

        if (inscritos >= curso.CupoMaximo)
        {
            TempData["Error"] = "El curso ya no tiene cupos disponibles.";
            return RedirectToAction("Detalle", new { id });
        }

        var existe = await _context.Matriculas
            .AnyAsync(m => m.CursoId == id && m.UsuarioId == userId);

        if (existe)
        {
            TempData["Error"] = "Ya estás inscrito en este curso.";
            return RedirectToAction("Detalle", new { id });
        }

        var matriculasUsuario = await _context.Matriculas
            .Include(m => m.Curso)
            .Where(m => m.UsuarioId == userId && m.Estado == "Pendiente")
            .ToListAsync();

        bool solapa = matriculasUsuario.Any(m =>
            (curso.HorarioInicio < m.Curso!.HorarioFin) &&
            (curso.HorarioFin > m.Curso.HorarioInicio)
        );

        if (solapa)
        {
            TempData["Error"] = "Ya tienes otro curso en el mismo horario.";
            return RedirectToAction("Detalle", new { id });
        }

        var matricula = new Matricula
        {
            CursoId = id,
            UsuarioId = userId,
            Estado = "Pendiente",
            FechaRegistro = DateTime.Now
        };

        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Inscripción realizada correctamente.";
        return RedirectToAction("Detalle", new { id });
    }
}
