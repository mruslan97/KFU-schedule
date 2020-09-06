using System;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Storage.Abstractions.Repository;
using Storage.Abstractions.UnitOfWork;
using Storage.EFCore.CreateTransactionFacade;
using Storage.EFCore.Repository.Impl;
using Storage.EFCore.UnitOfWork.Impl;

namespace Storage.EFCore.Extensions
{
    /// <summary>
    ///     Расширения для ServiceCollection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary> Добавить в коллецию зависимостей DataContext </summary>
        /// <param name="serviceCollection"> Коллекция зависимости </param>
        /// <param name="options"> Настройки к БД </param>
        public static IServiceCollection AddDataContext(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> options)
        {
            //contexts
            serviceCollection.AddDbContext<DataContext>(options, ServiceLifetime.Transient);

            serviceCollection.Add(new ServiceDescriptor(typeof(ICreateTransactionFacade), typeof(CreateTransactionFacade.Impl.CreateTransactionFacade), ServiceLifetime.Scoped));

            serviceCollection.Add(new ServiceDescriptor(typeof(IUnitOfWorkFactory), typeof(EFUnitOfWorkFactory), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(EFUnitOfWork), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IRepository<>), typeof(EFRepository<>), ServiceLifetime.Scoped));

            serviceCollection.Add(new ServiceDescriptor(typeof(IUnitOfWorkFactoryAsync), typeof(EfUnitOfWorkFactoryAsync), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IUnitOfWorkAsync), typeof(EFUnitOfWorkAsync), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IRepositoryAsync<>), typeof(EFRepositoryAsync<>), ServiceLifetime.Scoped));

            serviceCollection.Add(new ServiceDescriptor(typeof(ICurrentDbContext), provider => provider.GetService<DataContext>().Database.GetService<ICurrentDbContext>(), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IConcurrencyDetector), provider => provider.GetService<DataContext>().Database.GetService<IConcurrencyDetector>(), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IRawSqlCommandBuilder), provider => provider.GetService<DataContext>().Database.GetService<IRawSqlCommandBuilder>(), ServiceLifetime.Scoped));
            serviceCollection.Add(new ServiceDescriptor(typeof(IRelationalConnection), provider => provider.GetService<DataContext>().Database.GetService<IRelationalConnection>(), ServiceLifetime.Scoped));

            //connections
            serviceCollection.AddScoped(typeof(IDbConnection),
                provider => provider.GetService<DataContext>().Database.GetDbConnection());

            return serviceCollection;
        }

        public static IApplicationBuilder UseEntityFramework(this IApplicationBuilder appBuilder, bool needMigrate)
        {
            if (needMigrate)
            {
                var ctx = appBuilder.ApplicationServices.GetService<DataContext>();

                var listener = ((IInfrastructure<IServiceProvider>)ctx).GetService<DiagnosticSource>();

                (listener as DiagnosticListener).SubscribeWithAdapter(new CommandListener());

                ctx.Database.Migrate();
            }

            return appBuilder;
        }
    }
}