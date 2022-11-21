namespace WebAPIHuevos.DTOs
{
    public class EncargadoDTOConHuevos:GetEncargadoDTO
    {
        public List<HuevoDTO> Huevos { get; set; }
    }
}
