using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public IUpdateService UpdateService { get; set; }

        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeDb()
        {
            await ScheduleService.InitializeLocalStorage();

            return Ok("База университета успешно выгружена");
        }

        [HttpGet("update")]
        public async Task<IActionResult> UpdateDb()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await UpdateService.UpdateLocaleStorage();
            stopWatch.Stop();
            return Ok($"База предметов успешно обновлена, {stopWatch.Elapsed.TotalSeconds} сек.");
        }
    }
}