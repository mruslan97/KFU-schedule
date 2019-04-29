using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Storage.Abstractions.Interfaces;

namespace Storage.EFCore.Extensions
{
    /// <summary> Методы расширения для контекста базы данных </summary>
    public static class DataContextExtensions
    {
        /// <summary> Установить маппинги для обычных свойств IsDeleted, Created и т.д. </summary>
        /// <returns>The common mappings.</returns>
        /// <param name="modelBuilder">Model builder.</param>
        public static ModelBuilder InstallCommonMappings(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entity.ClrType;

                if (typeof(IPersistent).IsAssignableFrom(clrType))
                {
                    var method = ForPersistentMethodInfo.MakeGenericMethod(clrType);
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }

                if (typeof(IDeletablePersistent).IsAssignableFrom(clrType))
                {
                    var method = ForDeletableMethodInfo.MakeGenericMethod(clrType);
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }
            }

            modelBuilder.SetSnakeCaseNaming();

            return modelBuilder;
        }

        private static List<Type> GetGenericInterfaceTypes(this Type clrType, params Type[] types)
        {
            var genTypes = new List<Type>();
            foreach (Type intType in clrType.GetInterfaces())
            {
                if (intType.IsGenericType && types.Contains(intType.GetGenericTypeDefinition()))
                {
                    genTypes.Add(intType.GetGenericArguments()[0]);
                }
            }

            return genTypes;
        }

        /// <summary> Принимает соглашение в базе данных snake_case </summary>
        private static ModelBuilder SetSnakeCaseNaming(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Replace table names
                entity.Relational().TableName = ToSnakeCase(entity.Relational().TableName);

                // Replace column names
                foreach (var property in entity.GetProperties())
                {
                    property.Relational().ColumnName = ToSnakeCase(property.Name);
                }

                foreach (var key in entity.GetKeys())
                {
                    key.Relational().Name = ToSnakeCase(key.Relational().Name);
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.Relational().Name = ToSnakeCase(key.Relational().Name);
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.Relational().Name = ToSnakeCase(index.Relational().Name);
                }
            }


            return modelBuilder;
        }

        /// <summary>
        ///     Настроить первичный ключ и выставить значение по умолчанию для поля DateCreate
        /// </summary>
        /// <typeparam name="T">Тип, который настравивается</typeparam>
        private static ModelBuilder ForPersistent<T>(this ModelBuilder modelBuilder)
            where T : class, IPersistent
        {
            return modelBuilder.Entity<T>(self => {
                self.HasKey(x => x.Id);
                self.Property(x => x.Created).HasDefaultValueSql("NOW()");
                self.HasIndex(x => x.Id);
            });
        }

        /// <summary>
        ///     Настроить первичный ключ и выставить значение по умолчанию для поля DateCreate и IsDeleted.
        ///     ВАЖНО!!! Для таких объектов устанавливается глобальный фильтр, чтобы не возвращать удаленные объекты.
        /// </summary>
        /// <typeparam name="T">Тип, который настравивается</typeparam>
        private static ModelBuilder ForDeletable<T>(this ModelBuilder modelBuilder)
            where T : class, IDeletablePersistent
        {
            return modelBuilder.Entity<T>(self =>
            {
                self.Property(x => x.IsDeleted).HasDefaultValue(false);

                self.HasIndex(x => x.IsDeleted);

                self.HasQueryFilter(x => !x.IsDeleted);
            });
        }

        private static readonly MethodInfo ForPersistentMethodInfo = typeof(DataContextExtensions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == nameof(ForPersistent));
        
        private static readonly MethodInfo ForDeletableMethodInfo = typeof(DataContextExtensions).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == nameof(ForDeletable));

        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            var startUnderscores = Regex.Match(input, @"^_+");

            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}