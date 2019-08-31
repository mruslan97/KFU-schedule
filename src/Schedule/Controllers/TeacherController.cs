﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule.Controllers.Base;
using Schedule.Entities;

namespace Schedule.Controllers
{
    [Route("api/[controller]", Name = nameof(TeacherController)), Produces("application/json"), AllowAnonymous]
    public class TeacherController : BaseCrudController<Teacher>
    {

    }
}