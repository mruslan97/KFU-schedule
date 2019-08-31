using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.PostgreSql;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Schedule.Commands;
using Schedule.Extensions;
using Schedule.Mapper;
using Schedule.Models;
using Schedule.Services;
using Schedule.Services.Impl;
using Schedule.Services.Impl.QueryDecorators;
using Storage.EFCore.Extensions;
using Storage.Migrations;
using Swashbuckle.AspNetCore.Swagger;
using Vk.Bot.Framework.Abstractions;
using Vk.Bot.Framework.Extensions;
using VkNet;
using VkNet.Abstractions;
using VkNet.Categories;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Utils;


namespace Schedule
{
    public class Startup
    {
        /// <summary>
        ///     Путь к файлу конфига
        /// </summary>
        private readonly string ConfigPath = "appsettings.json";

        /// <summary>
        ///     Информация об окружении
        /// </summary>
        private IHostingEnvironment Env { get; }

        /// <summary>
        ///     Конфигурация приложения
        /// </summary>
        private IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            Env = env;
            var envConfigPath = Path.ChangeExtension(ConfigPath,
                $".{Env.EnvironmentName}{Path.GetExtension(ConfigPath)}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(ConfigPath, false, true)
                .AddJsonFile(envConfigPath, true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            //var culture = Configuration.GetValue<string>("DefaultCulture");

            //CultureInfo.CurrentCulture = new CultureInfo(culture);
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(typeof(ITimespanRepository<>), typeof(TimespanRepository<>));
            services.AddMvc()
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDataContext(builder =>
                builder.UseNpgsql(Configuration.GetConnectionString("Default"),
                        optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(DataContextFactory).Assembly.FullName))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging());
            services.AddHttpClient();
            services.Configure<DomainOptions>(Configuration.GetSection(nameof(DomainOptions)));
            services.AddTransient<IScheduleService, ScheduleService>();
            //ervices.AddTransient<IQueryFetchDecorator, FilterQueryDecorator>();
            services.AddTransient<IQueryFetchDecorator, SearchQueryDecorator>();
            services.AddTransient<IQueryFetchDecorator, OrderByQueryDecorator>();
            services.AddScoped<IUpdateService, UpdateService>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Kpfu schedule API", Version = "v1" });
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                foreach (var comment in Directory.GetFiles(basePath, "*.xml"))
                {
                    c.IncludeXmlComments(comment, true);
                }
            });

            services.AddSingleton<IVkApi>(sp =>
            {
                var api = new VkApi(services);

                api.Authorize(new ApiAuthParams
                {
                    AccessToken = Configuration.GetSection("VkToken").Get<string>(),
                    Settings = Settings.Messages

                });

                return api;
            });

            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile(Configuration.GetSection(nameof(DomainOptions)).Get<DomainOptions>()));
            });
            services.AddSingleton(mappingConfig.CreateMapper());
            services.AddVkBot<KpfuBot>(Configuration.GetSection("VkOptions"))
                //.AddUpdateHandler<HelpCommand>()
                .AddUpdateHandler<MainMenuCommand>()
                .AddUpdateHandler<HelloCommand>()
                .AddUpdateHandler<SetupGroupCommand>()
                .AddUpdateHandler<TodayCommand>()
                .AddUpdateHandler<TomorrowCommand>()
                .AddUpdateHandler<WeekCommand>()
                .AddUpdateHandler<TeacherSearchCommand>()
                .Configure();

            services.AddHangfire(x => x.UseMemoryStorage());
            

            return services.BuildDryIoc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration config)
        {
            bool enableAutoMigration =
                bool.TryParse(config.GetSection("EnableAutoMigration")?.Value, out enableAutoMigration) && enableAutoMigration;

            app.UseEntityFramework(needMigrate: enableAutoMigration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            app.UseSwagger();
            app.UseVkBot<KpfuBot>();
            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate<IUpdateService>(u => u.UpdateLocaleStorage(), Cron.Daily);
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
