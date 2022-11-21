using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.EntityFrameworkCore;
using WebAPIHuevos.DTOs;
using WebAPIHuevos.Entidades;

namespace WebAPIHuevos.Controllers
{
    [ApiController]
    [Route("huevos/{huevoId:int}/cursos")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CursosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CursosController(ApplicationDbContext dbContext, IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<List<CursoDTO>>> Get(int huevoId)
        {
            var existeHuevo = await dbContext.Huevos.AnyAsync(huevoDB => huevoDB.Id == huevoId);
            if (!existeHuevo)
            {
                return NotFound();
            }

            var cursos = await dbContext.Cursos.Where(cursoDB => cursoDB.HuevoId == huevoId).ToListAsync();

            return mapper.Map<List<CursoDTO>>(cursos);
        }

        [HttpGet("{id:int}", Name = "obtenerCurso")]
        public async Task<ActionResult<CursoDTO>> GetById(int id)
        {
            var curso = await dbContext.Cursos.FirstOrDefaultAsync(cursoDB => cursoDB.Id == id);

            if (curso == null)
            {
                return NotFound();
            }

            return mapper.Map<CursoDTO>(curso);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post(int huevoId, CursoCreacionDTO cursoCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();

            var email = emailClaim.Value;

            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeHuevo = await dbContext.Huevos.AnyAsync(huevoDB => huevoDB.Id == huevoId);
            if (!existeHuevo)
            {
                return NotFound();
            }

            var curso = mapper.Map<Cursos>(cursoCreacionDTO);
            curso.HuevoId = huevoId;
            curso.UsuarioId = usuarioId;
            dbContext.Add(curso);
            await dbContext.SaveChangesAsync();

            var cursoDTO = mapper.Map<CursoDTO>(curso);

            return CreatedAtRoute("obtenerCurso", new { id = curso.Id, huevoId = huevoId }, cursoDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int huevoId, int id, CursoCreacionDTO cursoCreacionDTO)
        {
            var existeHuevo = await dbContext.Huevos.AnyAsync(huevoDB => huevoDB.Id == huevoId);
            if (!existeHuevo)
            {
                return NotFound();
            }

            var existeCurso = await dbContext.Cursos.AnyAsync(cursoDB => cursoDB.Id == id);
            if (!existeCurso)
            {
                return NotFound();
            }

            var curso = mapper.Map<Cursos>(cursoCreacionDTO);
            curso.Id = id;
            curso.HuevoId = huevoId;

            dbContext.Update(curso);
            await dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
