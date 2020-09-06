using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Schedule.Controllers.Base;
using Schedule.Entities;
using Schedule.Entities.Kpfu;
using Schedule.Services;
using  vm = Schedule.Models;

namespace Schedule.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize(Policy = "ApiKeyPolicy")]
    public class ScheduleController : Controller
    {
        public IScheduleService ScheduleService { get; set; }
        
        public IOptions<vm.DomainOptions> Options { get; set; }


        [HttpGet("initialize")]
        public async Task<IActionResult> InitializeDb()
        {
            await ScheduleService.InitializeLocalStorage();

            return Ok("База университета успешно выгружена");
        }

        [HttpGet("updateLocalDb")]
        public async Task<IActionResult> UpdateLocalDb()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await ScheduleService.UpdateLocalDb();
            stopWatch.Stop();
            return Ok($"База успешно обновлена, {stopWatch.Elapsed.TotalSeconds} сек. Текущие настройки: год {Options.Value.Year} // семестр {Options.Value.Semester}");
        }

        [HttpGet("updateSubjects")]
        public async Task<IActionResult> UpdateSubjects()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await ScheduleService.UpdateSubjects();
            stopWatch.Stop();
            return Ok($"Таблица предметов успешно обновлена, {stopWatch.Elapsed.TotalSeconds} сек.");
        }
    }
}