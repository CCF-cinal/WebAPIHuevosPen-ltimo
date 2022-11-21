using System.ComponentModel.DataAnnotations;
using WebAPIHuevos.Validaciones;

namespace WebAPIHuevos.Entidades
{
    public class Encargado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 10, ErrorMessage = "El campo {0} solo puede tener hasta 10 caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }
        public List<EncargadoHuevo> EncargadoHuevo { get; set; }
    }
}
