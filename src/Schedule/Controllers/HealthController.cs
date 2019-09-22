using System;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Schedule.Controllers
{
    public class HealthController : Controller
    {
        /// <summary> Если метод вернет ответ, то сервер работает </summary>
        [HttpGet("/")]
        public string Index()
        {
            return $"{DateTime.Now.ToString(CultureInfo.CurrentCulture)} Сервер работает в штатном режиме";
        }
    }
}