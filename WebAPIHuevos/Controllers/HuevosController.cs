using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIHuevos.DTOs;
using WebAPIHuevos.Entidades;

namespace WebAPIHuevos.Controllers
{
    [ApiController]
    [Route("huevos")]
    public class HuevosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public HuevosController(ApplicationDbContext context, IMapper mapper)
        {
            this.dbContext = context;
            this.mapper = mapper;
        }

        [HttpGet]
        [HttpGet("/listadoHuevo")]
        public async Task<ActionResult<List<Huevo>>> GetAll()
        {
            return await dbContext.Huevos.ToListAsync();
        }

        [HttpGet("{id:int}", Name = "obtenerHuevo")]
        public async Task<ActionResult<HuevoDTOConEncargados>> GetById(int id)
        {
            var huevo = await dbContext.Huevos
                .Include(huevoDB => huevoDB.EncargadoHuevo)
                .ThenInclude(encargadoHuevoDB => encargadoHuevoDB.Encargado)
                .Include(cursoDB => cursoDB.Cursos)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (huevo == null)
            {
                return NotFound();
            }

            huevo.EncargadoHuevo = huevo.EncargadoHuevo.OrderBy(x => x.Orden).ToList();

            return mapper.Map<HuevoDTOConEncargados>(huevo);
        }

        [HttpPost]
        public async Task<ActionResult> Post(HuevoCreacionDTO huevoCreacionDTO)
        {

            if (huevoCreacionDTO.EncargadosIds == null)
            {
                return BadRequest("No se puede crear un huevo sin encargados.");
            }

            var encargadosIds = await dbContext.Encargados
                .Where(encargadoBD => huevoCreacionDTO.EncargadosIds.Contains(encargadoBD.Id)).Select(x => x.Id).ToListAsync();

            if (huevoCreacionDTO.EncargadosIds.Count != encargadosIds.Count)
            {
                return BadRequest("No existe uno de los encagados enviados");
            }

            var huevo = mapper.Map<Huevo>(huevoCreacionDTO);

            OrdenarPorEncagados(huevo);

            dbContext.Add(huevo);
            await dbContext.SaveChangesAsync();

            var huevoDTO = mapper.Map<HuevoDTO>(huevo);

            return CreatedAtRoute("obtenerHuevo", new { id = huevo.Id }, huevoDTO);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, HuevoCreacionDTO huevoCreacionDTO)
        {
            var huevoDB = await dbContext.Huevos
                .Include(x => x.EncargadoHuevo)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (huevoDB == null)
            {
                return NotFound();
            }

            huevoDB = mapper.Map(huevoCreacionDTO, huevoDB);

            OrdenarPorEncagados(huevoDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Huevos.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            


            dbContext.Remove(new Huevo { Id = id });
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        private void OrdenarPorEncagados(Huevo huevo)
        {
            if (huevo.EncargadoHuevo != null)
            {
                for (int i = 0; i < huevo.EncargadoHuevo.Count; i++)
                {
                    huevo.EncargadoHuevo[i].Orden = i;
                }
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<HuevoPatchDTO> patchDocument)
        {
            if (patchDocument == null) { return BadRequest(); }

            var huevoDB = await dbContext.Huevos.FirstOrDefaultAsync(x => x.Id == id);

            if (huevoDB == null) { return NotFound(); }

            var huevoDTO = mapper.Map<HuevoPatchDTO>(huevoDB);

            patchDocument.ApplyTo(huevoDTO);

            var isValid = TryValidateModel(huevoDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(huevoDTO, huevoDB);

            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
