using System.ComponentModel.DataAnnotations;

namespace Parcial.Models
{
    public class Curso
    {
        public int Id { get; set; }

        [Required, StringLength(10)]
        public string Codigo { get; set; } = "";

        [Required, StringLength(100)]
        public string Nombre { get; set; } = "";

        [Range(1, 20, ErrorMessage = "Los créditos deben ser positivos.")]
        public int Creditos { get; set; }

        [Range(1, 200, ErrorMessage = "El cupo debe ser mayor que cero.")]
        public int CupoMaximo { get; set; }

        public TimeSpan HorarioInicio { get; set; }
        public TimeSpan HorarioFin { get; set; }

        public bool Activo { get; set; } = true;

        public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();
    }
}
