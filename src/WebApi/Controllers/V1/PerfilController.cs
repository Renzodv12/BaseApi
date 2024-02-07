using Core.Entities.Models;
using Core.Interfaces;
using Infra.DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilController : BaseController<Perfil>
    {

        public PerfilController(ApiDbContext apiDbContext, ICacheManager<Perfil> cacheManager) : base(apiDbContext, cacheManager)
        {
        }

    }
}
