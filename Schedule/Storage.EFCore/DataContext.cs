using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Schedule.Entities;
using Storage.EFCore.Extensions;

namespace Storage.EFCore
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Subject> Subjects { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<Group> Groups { get; set; }
        /// <summary>
        ///     Конструктор класса без конфигурации
        /// </summary>
        public DataContext() { }

        /// <summary>
        ///     Конструктор, который используется IOC-ом
        /// </summary>
        /// <param name="options"></param>
        [UsedImplicitly] public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        /// <summary>
        ///     Настройка маппингов, индексов и т.д
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating([NotNull] ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity
                    .HasIndex(x => x.KpfuId)
                    .IsUnique();
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity
                    .HasIndex(x => x.GroupName)
                    .IsUnique();
            });

            modelBuilder.InstallCommonMappings();

            base.OnModelCreating(modelBuilder);
        }
    }
}