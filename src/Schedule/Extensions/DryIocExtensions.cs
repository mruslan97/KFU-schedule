using System;
using System.ComponentModel;
using DryIoc;
using DryIoc.MefAttributedModel;
using DryIoc.Microsoft.DependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Container = DryIoc.Container;
using IContainer = DryIoc.IContainer;

namespace Schedule.Extensions
{
    /// <summary>
    ///     Класс расширений для DryIoc контейнера
    /// </summary>
    public static class DryIocExtensions
    {
        /// <summary>
        ///     Добавляет опцию инициализации свойств в контроллерах
        /// </summary>
        /// <param name = "container" > </param>
        /// <returns> </returns>
        [PublicAPI]
        public static IContainer AddPropertyInjectionsForControllers(this IContainer container)
        {
            return container.With(rules => rules.With(
                propertiesAndFields: request => FindServiceTypes(request.ServiceType)
                    ? PropertiesAndFields.Auto(request)
                    : null));
        }

        /// <summary>
        ///     Добавление DryIoc по умолчанию
        /// </summary>
        /// <param name = "services" > </param>
        /// <returns> </returns>
        public static IServiceProvider BuildDryIoc(this IServiceCollection services)
        {
            return new Container(rules =>
                    rules.WithFactorySelector(Rules.SelectLastRegisteredFactory()))
                .WithMefAttributedModel()
                .AddPropertyInjectionsForControllers()
                .WithDependencyInjectionAdapter(services);
        }

        private static bool FindServiceTypes(Type type)
        {
            return type.Name.EndsWith("Controller", StringComparison.Ordinal)
                   || type.Namespace.Contains("Schedule.Services", StringComparison.Ordinal);
        }
    }
}