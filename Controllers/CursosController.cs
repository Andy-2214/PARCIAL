using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Parcial.Data;
using Parcial.Models;

public class CursosController : Controller
{
    private readonly ApplicationDbContext _context;

    public CursosController(ApplicationDbContext context)
    {
        _context = context;
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
}
