using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIHuevos.DTOs;
using WebAPIHuevos.Entidades;

namespace WebAPIHuevos.Controllers
{
    [ApiController]
    [Route("encargados")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class EncargadosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public EncargadosController(ApplicationDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.dbContext = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        //[HttpGet("configuraciones")]
        //public ActionResult<string> ObtenerConfiguracion()
        //{
        //    var configDirecta = configuration["apellido"];
        //    var configMapeada = configuration["connectionStrings:defaultConnection"];

        //    Console.WriteLine("Se obtiene valor de configuracion directo: " + configDirecta);
        //    Console.WriteLine("Se obtiene valor de configuracion mappeado: " + configMapeada);
        //    return Ok();
        //}

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetEncargadoDTO>>> Get()
        {
            var encargados = await dbContext.Encargados.ToListAsync();
            return mapper.Map<List<GetEncargadoDTO>>(encargados);
        }


        [HttpGet("{id:int}", Name = "obtenerencargado")] 
        public async Task<ActionResult<EncargadoDTOConHuevos>> Get(int id)
        {
            var encargado = await dbContext.Encargados
                .Include(encargadoDB => encargadoDB.EncargadoHuevo)
                .ThenInclude(encargadoHuevoDB => EncargadoHuevoDB.Huevo)
                .FirstOrDefaultAsync(encargadoBD => EncargadoBD.Id == id);

            if (encargado == null)
            {
                return NotFound();
            }

            return mapper.Map<EncargadoDTOConHuevos>(encargado);

        }

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<GetEncargadoDTO>>> Get([FromRoute] string nombre)
        {
            var encargados = await dbContext.Encargados.Where(encargadoBD => encargadoBD.Nombre.Contains(nombre)).ToListAsync();

            return mapper.Map<List<GetEncargadoDTO>>(encargados);

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] EncargadoDTO encargadoDto)
        {
            //Ejemplo para validar desde el controlador con la BD con ayuda del dbContext

            var existeEncargadoMismoNombre = await dbContext.Encargados.AnyAsync(x => x.Nombre == encargadoDto.Nombre);

            if (existeEncargadoMismoNombre)
            {
                return BadRequest($"Ya existe un autor con el nombre {encargadoDto.Nombre}");
            }

            var encargado = mapper.Map<Encargado>(encargadoDto);

            dbContext.Add(encargado);
            await dbContext.SaveChangesAsync();

            var encargadoDTO = mapper.Map<GetEncargadoDTO>(encargado);

            return CreatedAtRoute("obtenerencargado", new { id = encargado.Id }, encargadoDTO);
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> Put(EncargadoDTO encargadoCreacionDTO, int id)
        {
            var exist = await dbContext.Encargados.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound();
            }

            var encargado = mapper.Map<Encargado>(encargadoCreacionDTO);
            encargado.Id = id;

            dbContext.Update(encargado);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await dbContext.Encargados.AnyAsync(x => x.Id == id);
            if (!exist)
            {
                return NotFound("El Recurso no fue encontrado.");
            }

            dbContext.Remove(new Encargado()
            {
                Id = id
            });
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
