using Microsoft.AspNetCore.Identity;

namespace WebAPIHuevos.Entidades
{
    public class Cursos
    {
        public int Id { get; set; }
        public string Contenido { get; set; }

        public int HuevoId { get; set; }

        public Huevo Huevo { get; set; }

        public string UsuarioId { get; set; }

        public IdentityUser Usuario { get; set; }
    }
}
