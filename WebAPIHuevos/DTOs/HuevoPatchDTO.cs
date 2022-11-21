using System.ComponentModel.DataAnnotations;
using WebAPIHuevos.Validaciones;

namespace WebAPIHuevos.DTOs
{
    public class HuevoPatchDTO
    {
        [Required]
        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} solo puede tener hasta 250 caracteres")]
        [PrimeraLetraMayuscula]
        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
