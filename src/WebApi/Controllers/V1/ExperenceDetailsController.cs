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
    public class ExperenceDetailsController : BaseController<ExperenceDetails>
    {

        public ExperenceDetailsController(ApiDbContext apiDbContext, ICacheManager<ExperenceDetails> cacheManager) : base(apiDbContext, cacheManager)
        {
        }

    }
}
