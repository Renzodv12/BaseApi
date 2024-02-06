using Core.Entities;
using Infra.DBManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    public class BaseController<T> : ControllerBase where T : Entity
    {
        private readonly ApiDbContext _apiDbContext;

        public BaseController(ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var userId = getacount();
            if (!string.IsNullOrEmpty(userId))
            {
                return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());

            }
            else
            {
                return BadRequest();
            }

        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var userId = getacount();
            if (!string.IsNullOrEmpty(userId))
            {
                return Ok(await _apiDbContext.Set<T>().Where(x => x.UserId == userId && x.Id == id).FirstOrDefaultAsync());

            }
            else
            {
                return BadRequest();
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(T model)
        {
            if (ModelState.IsValid)
            {
                var userId = getacount();
                if (!string.IsNullOrEmpty(userId))
                {

                    model.UserId = userId;
                    await _apiDbContext.AddAsync<T>(model);
                    await _apiDbContext.SaveChangesAsync();
                    return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());

                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();

            }


        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put(T model)
        {
            if (ModelState.IsValid)
            {
                var userId = getacount();
                if (!string.IsNullOrEmpty(userId))
                {
                    var modelvalidator = await _apiDbContext.Set<T>().FindAsync(model.Id);

                    if (modelvalidator != null && modelvalidator?.UserId == userId)
                    {

                        model.Fmodificacion = DateTime.Now;
                        _apiDbContext.Update<T>(model);
                        await _apiDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return BadRequest("Objeto no te pertenece");
                    }
                }
                return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var userId = getacount();
                if (!string.IsNullOrEmpty(userId))
                {
                    var model = await _apiDbContext.Set<T>().Where(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();
                    if (model != null)
                    {

                        _apiDbContext.Remove<T>(model);
                        await _apiDbContext.SaveChangesAsync();
                    }
                }
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }
        private string? getacount()
        {
            // Accede al principal del usuario actual (usuario autenticado)
            var principal = HttpContext.User;

            // Busca la claim (reclamación) correspondiente al email
            var emailClaim = principal?.FindFirst("Id");

            // Si la claim del email existe, obtén su valor
            if (emailClaim != null)
            {
                return emailClaim.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
