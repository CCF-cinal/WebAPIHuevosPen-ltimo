using AutoMapper;
using WebAPIHuevos.DTOs;
using WebAPIHuevos.Entidades;

namespace WebAPIHuevos.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<EncargadoDTO, Encargado>();
            CreateMap<Encargado, GetEncargadoDTO>();
            CreateMap<Encargado, EncargadoDTOConHuevos>()
                .ForMember(encargadoDTO => encargadoDTO.Huevos, opciones => opciones.MapFrom(MapEncargadoDTOHuevos));
            CreateMap<HuevoCreacionDTO, Huevo>()
                .ForMember(huevo => huevo.EncargadoHuevo, opciones => opciones.MapFrom(MapEncargadoHuevo));
            CreateMap<Huevo, HuevoDTO>();
            CreateMap<Huevo, HuevoDTOConEncargados>()
                .ForMember(huevoDTO => huevoDTO.Encargados, opciones => opciones.MapFrom(MapHuevoDTOEncargados));
            CreateMap<HuevoPatchDTO, Huevo>().ReverseMap();
            CreateMap<CursoCreacionDTO, Cursos>();
            CreateMap<Cursos, CursoDTO>();
        }

        private List<HuevoDTO> MapEncargadoDTOHuevos(Encargado encargado, GetEncargadoDTO getEncargadoDTO)
        {
            var result = new List<HuevoDTO>();

            if (encargado.EncargadoHuevo == null) { return result; }

            foreach (var encargadoHuevo in encargado.EncargadoHuevo)
            {
                result.Add(new HuevoDTO()
                {
                    Id = encargadoHuevo.HuevoId,
                    Estado = encargadoHuevo.Huevo.Estado
                });
            }

            return result;
        }

        private List<GetEncargadoDTO> MapHuevoDTOEncargados(Huevo huevo, HuevoDTO huevoDTO)
        {
            var result = new List<GetEncargadoDTO>();

            if (huevo.EncargadoHuevo == null)
            {
                return result;
            }

            foreach (var encargadohuevo in huevo.EncargadoHuevo)
            {
                result.Add(new GetEncargadoDTO()
                {
                    Id = encargadohuevo.EncargadoId,
                    Nombre = encargadohuevo.Encargado.Nombre
                });
            }

            return result;
        }

        private List<EncargadoHuevo> MapEncargadoHuevo(HuevoCreacionDTO huevoCreacionDTO, Huevo huevo)
        {
            var resultado = new List<EncargadoHuevo>();

            if (huevoCreacionDTO.EncargadosIds == null) { return resultado; }
            foreach (var encargadoId in huevoCreacionDTO.EncargadosIds)
            {
                resultado.Add(new EncargadoHuevo() { EncargadoId = encargadoId });
            }
            return resultado;
        }
    }
}
