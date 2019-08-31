using System;
using System.Diagnostics;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Storage.EFCore;

namespace Storage.Migrations
{
    /// <summary> Используется мигратором для создания контекста БД </summary>
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        /// <summary> Создать контекст БД </summary>
        /// <param name="args"> Аргументы командной строки (не используется)</param>
        /// <returns> Контекст БД </returns>
        public DataContext CreateDbContext(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("ru-RU");
            var cfg = new ConfigurationBuilder()
                .AddJsonFile("migrationSettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<DataContext>();
            builder.UseNpgsql(cfg.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(DataContextFactory).Assembly.FullName));

            var ctx = new DataContext(builder.Options);

            var listener = ((IInfrastructure<IServiceProvider>)ctx).GetService<DiagnosticSource>();

            (listener as DiagnosticListener).SubscribeWithAdapter(new CommandListener());

            return ctx;
        }
    }
}