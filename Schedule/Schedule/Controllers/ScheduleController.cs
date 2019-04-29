using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Schedule.Controllers.Base;
using Schedule.Entities;
using Schedule.Entities.Kpfu;
using Schedule.Services;
using  vm = Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : Controller
    {
        public IScheduleService ScheduleService { get; set; }

        [HttpGet("test")]
        public async Task<IActionResult> InitializeDb()
        {
            await ScheduleService.InitializeLocalStorage();

            return Ok("База университета успешно выгружена");
        }
    }
}