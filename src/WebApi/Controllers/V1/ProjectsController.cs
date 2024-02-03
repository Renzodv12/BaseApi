using Core.Entities.Models;
using Infra.DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : BaseController
    {
        private readonly ApiDbContext _apiDbContext;

        public ProjectsController(ApiDbContext apiDbContext)
        {
            _apiDbContext = apiDbContext;
        }

        [HttpGet]
        public IActionResult Get()
        {

            return Json(_apiDbContext.Projects.ToList());

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Json(await _apiDbContext.Projects.Where(x => x.Id == id).FirstOrDefaultAsync());
        }
        [HttpPost]
        public async Task<IActionResult> Post(Projects _projects)
        {
            if (ModelState.IsValid)
            {
                await _apiDbContext.Projects.AddAsync(_projects);
                await _apiDbContext.SaveChangesAsync();
            }
            return Json(_apiDbContext.Projects.ToList());
        }

        [HttpPut]
        public async Task<IActionResult> Put(Projects _projects)
        {
            if (ModelState.IsValid)
            {
                _projects.Fmodificacion = DateTime.Now;
                _apiDbContext.Update<Projects>(_projects);
                await _apiDbContext.SaveChangesAsync();
            }
            return Json(_apiDbContext.Projects.ToList());
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var _projects = await _apiDbContext.Projects.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (_projects != null)
                {

                    _apiDbContext.Projects.Remove(_projects);
                    await _apiDbContext.SaveChangesAsync();
                }
            }
            return Json(_apiDbContext.Projects.ToList());
        }


    }
}
