using Core.Entities;
using Core.Interfaces;
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
        private readonly ICacheManager<T> _cacheManager;

        public BaseController(ApiDbContext apiDbContext, ICacheManager<T> cacheManager)
        {
            _apiDbContext = apiDbContext;
            _cacheManager = cacheManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = getacount();
            if (!string.IsNullOrEmpty(userId))
            {
                string key = typeof(T).Name + "." + userId;
                return Ok(_cacheManager.GetAsync(key, async () =>
                {
                    return await _apiDbContext.Set<T>().Where(x => x.UserId == userId).ToListAsync();
                }));

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
                    string key = typeof(T).Name + "." + userId;
                    await _cacheManager.RemoveAsync(key);
                    return await Get();

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
                string key = typeof(T).Name + "." + userId;
                await _cacheManager.RemoveAsync(key);
                return await Get();
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
                string key = typeof(T).Name + "." + userId;
                await _cacheManager.RemoveAsync(key);
            }
            return await Get();


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
