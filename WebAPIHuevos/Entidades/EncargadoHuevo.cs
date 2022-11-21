namespace WebAPIHuevos.Entidades
{
    public class EncargadoHuevo
    {
        public int EncargadoId { get; set; }
        public int HuevoId { get; set; }
        public int Orden { get; set; }
        public Encargado Encargado { get; set; }
        public Huevo Huevo { get; set; }
    }
}
