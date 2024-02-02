using Core.Entities.Models;
using Infra.DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : BaseController
    {
        private readonly ApiDbContext _apiDbContext;

        public PerfilController(ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {

            return Json(_apiDbContext.Perfil.ToList());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Json(await _apiDbContext.Perfil.Where(x => x.Id == id).FirstOrDefaultAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Post(Perfil _perfil)
        {
            if (ModelState.IsValid)
            {
                await _apiDbContext.Perfil.AddAsync(_perfil);
                await _apiDbContext.SaveChangesAsync();
            }
            return Json(_apiDbContext.Perfil.ToList());
        }

        [HttpPut]
        public async Task<IActionResult> Put(Perfil _perfil)
        {
            if (ModelState.IsValid)
            {
                _perfil.Fmodificacion = DateTime.Now;
                _apiDbContext.Update<Perfil>(_perfil);
                await _apiDbContext.SaveChangesAsync();
            }
            return Json(_apiDbContext.Perfil.ToList());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var _perfil = await _apiDbContext.Perfil.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (_perfil != null)
                {

                    _apiDbContext.Perfil.Remove(_perfil);
                    await _apiDbContext.SaveChangesAsync();
                }
            }
            return Json(_apiDbContext.Perfil.ToList());
        }


    }
}
