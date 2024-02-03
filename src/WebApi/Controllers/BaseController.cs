using Core.Entities;
using Infra.DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get()
        {

            return Ok(_apiDbContext.Set<T>().ToList());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await _apiDbContext.Set<T>().FindAsync(id));
        }
        [HttpPost]
        public async Task<IActionResult> Post(T model)
        {
            if (ModelState.IsValid)
            {
                await _apiDbContext.AddAsync<T>(model);
                await _apiDbContext.SaveChangesAsync();
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }

        [HttpPut]
        public async Task<IActionResult> Put(T model)
        {
            if (ModelState.IsValid)
            {
                model.Fmodificacion = DateTime.Now;
                _apiDbContext.Update<T>(model);
                await _apiDbContext.SaveChangesAsync();
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var model = await _apiDbContext.Set<T>().FindAsync(id);
                if (model != null)
                {

                    _apiDbContext.Remove<T>(model);
                    await _apiDbContext.SaveChangesAsync();
                }
            }
            return Ok(_apiDbContext.Set<T>().ToList());
        }

    }
}
