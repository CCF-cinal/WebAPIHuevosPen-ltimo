using System.ComponentModel.DataAnnotations;

namespace WebAPIHuevos.DTOs
{
    public class EditarAdminDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
