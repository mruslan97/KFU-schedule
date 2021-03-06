﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.Controllers.Base;
using Schedule.Entities;

namespace Schedule.Controllers
{
    [Route("api/[controller]", Name = nameof(SubjectController)), Produces("application/json"), Authorize(Policy = "ApiKeyPolicy")]
    public class SubjectController : BaseCrudController<Subject>
    {
        
    }
}