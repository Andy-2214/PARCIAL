using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Parcial.Data;
using Parcial.Models;

[Authorize(Roles = "Coordinador")]
public class CoordinadorController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoordinadorController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var cursos = await _context.Cursos.ToListAsync();
        return View(cursos);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(Curso curso)
    {
        if (ModelState.IsValid)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    public async Task<IActionResult> Editar(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null)
            return NotFound();

        return View(curso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Curso curso)
    {
        if (id != curso.Id) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(curso);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(curso);
    }

    [HttpGet]
public async Task<IActionResult> Desactivar(int id)
    {
        var curso = await _context.Cursos.FindAsync(id);
        if (curso == null) return NotFound();

        curso.Activo = false;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

   public async Task<IActionResult> MatriculasCurso(int cursoId)
{
    var curso = await _context.Cursos
        .Include(c => c.Matriculas)
        .FirstOrDefaultAsync(c => c.Id == cursoId);

    if (curso == null) return NotFound();

    return View(curso);
}

   [HttpGet]
public async Task<IActionResult> Confirmar(int id)
{
    var matricula = await _context.Matriculas.FindAsync(id);
    if (matricula == null) return NotFound();

    matricula.Estado = "Confirmada";
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(MatriculasCurso), new { cursoId = matricula.CursoId });
}

[HttpGet]
public async Task<IActionResult> Cancelar(int id)
{
    var matricula = await _context.Matriculas.FindAsync(id);
    if (matricula == null) return NotFound();

    matricula.Estado = "Cancelada";
    await _context.SaveChangesAsync();

  return RedirectToAction(nameof(MatriculasCurso), new { cursoId = matricula.CursoId });
}
}
