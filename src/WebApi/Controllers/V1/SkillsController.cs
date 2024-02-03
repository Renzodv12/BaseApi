﻿using Core.Entities.Models;
using Infra.DBManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : BaseController<Skills>
    {

        public SkillsController(ApiDbContext apiDbContext) : base(apiDbContext)
        {
        }

    }
}
