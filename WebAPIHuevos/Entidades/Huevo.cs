using System.ComponentModel.DataAnnotations;
using WebAPIHuevos.Validaciones;

namespace WebAPIHuevos.Entidades
{
    public class Huevo
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} solo puede tener hasta 250 caracteres")]
        [PrimeraLetraMayuscula]
        public string Estado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public List<Cursos> Cursos { get; set; }
        public List<EncargadoHuevo> EncargadoHuevo { get; set; }
    }
}
