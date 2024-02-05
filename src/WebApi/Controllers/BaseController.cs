using System.Security.Claims;
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


        [HttpGet]
        public IActionResult Get(bool onlyMe = false)
        {
            if (onlyMe)
            {
                string? userId = getacount();
                if (!string.IsNullOrEmpty(userId))
                {
                    return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());
                }
                else
                {
                    return BadRequest("Usuario no encontrado o no valido");
                }
            }

            return Ok(_apiDbContext.Set<T>().ToList());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, bool onlyMe = false)
        {
            if (onlyMe)
            {
                string? userId = getacount();
                if (!string.IsNullOrEmpty(userId))
                {
                    return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId && x.Id == id).ToList());
                }
                else
                {
                    return BadRequest("Usuario no encontrado o no valido");
                }
            }
            return Ok(await _apiDbContext.Set<T>().FindAsync(id));
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post(T model, bool onlyMe = false)
        {
            if (ModelState.IsValid)
            {
                if (onlyMe)
                {
                    string? userId = getacount();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        model.UserId = userId;

                        await _apiDbContext.AddAsync<T>(model);
                        await _apiDbContext.SaveChangesAsync();
                        return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());
                    }
                    else
                    {
                        return BadRequest("Usuario no encontrado o no valido");
                    }

                }
                await _apiDbContext.AddAsync<T>(model);
                await _apiDbContext.SaveChangesAsync();
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put(T model, bool onlyMe = false)
        {
            if (ModelState.IsValid)
            {
                if (onlyMe)
                {
                    string? userId = getacount();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        model.UserId = userId;
                        return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());
                    }
                    else
                    {
                        return BadRequest("Usuario no encontrado o no valido");
                    }

                }
                model.Fmodificacion = DateTime.Now;
                _apiDbContext.Update<T>(model);
                await _apiDbContext.SaveChangesAsync();
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id, bool onlyMe = false)
        {
            if (ModelState.IsValid)
            {

                if (onlyMe)
                {
                    string? userId = getacount();
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var model = await _apiDbContext.Set<T>().Where(x => x.Id == id && x.UserId == userId).FirstOrDefaultAsync();

                        if (model != null)
                        {
                            _apiDbContext.Remove<T>(model);
                            await _apiDbContext.SaveChangesAsync();
                        }
                        return Ok(_apiDbContext.Set<T>().Where(x => x.UserId == userId).ToList());
                    }
                    else
                    {
                        return BadRequest("Usuario no encontrado o no valido");
                    }

                }
                else
                {
                    var model = await _apiDbContext.Set<T>().FindAsync(id);

                    if (model != null)
                    {
                        _apiDbContext.Remove<T>(model);
                        await _apiDbContext.SaveChangesAsync();
                    }
                    return Ok(_apiDbContext.Set<T>().ToList());

                }
            }
            return BadRequest();
        }


        private string? getacount()
        {
            // Accede al principal del usuario actual (usuario autenticado)
            var principal = HttpContext.User as ClaimsPrincipal;

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
