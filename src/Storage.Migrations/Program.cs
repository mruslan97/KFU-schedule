using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Storage.Migrations
{
    /// <summary> Тип в котором есть метод, точки входа в програму </summary>
    public static class Program
    {
        /// <summary> Точка входа в программу </summary>
        /// <param name="args"> Аргументы коммандной строки </param>
        /// <returns> Задача </returns>
        [STAThread]
        public static async Task Main(string[] args)
        {
            var factory = new DataContextFactory();

            var context = factory.CreateDbContext(args);

            await context.Database.MigrateAsync();
        }
    }
}
