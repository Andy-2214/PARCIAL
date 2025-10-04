using System.ComponentModel.DataAnnotations;

namespace Parcial.Models
{
    public class Matricula
    {
        public int Id { get; set; }
        public int CursoId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public string Estado { get; set; } = "Pendiente";

        public Curso? Curso { get; set; }
    }
}
