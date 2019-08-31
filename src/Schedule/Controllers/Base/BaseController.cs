using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using vm = Schedule.Models;
namespace Schedule.Controllers.Base
{
    /// <summary> Базовый контроллер </summary>
    public abstract class BaseController : Controller
    {
        /// <summary> Сохранить в свойстве значнеия query parameters </summary>
        protected void SetWhereParams(vm.PageListRequest request, params string[] exclude)
        {
            if (Request?.Query == null)
            {
                return;
            }

            request.Where = Request.Query
                .Where(x => exclude.All(z => !string.Equals(x.Key, z, StringComparison.InvariantCultureIgnoreCase)))
                .ToDictionary(
                    x => x.Key,
                    x => new vm.QueryParameter(x.Value),
                    StringComparer.InvariantCultureIgnoreCase);
        }
    }
}