namespace WebAPIHuevos.DTOs
{
    public class HuevoDTO
    {
        public int Id { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }
        public List<CursoDTO> Cursos { get; set; }
    }
}
