using UserService.Models;
using Microsoft.EntityFrameworkCore;


namespace UserService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<EntityListMetadataModel> EntityListMetadataModels { get; set; }
        public DbSet<EntityColumnListMetadataModel> EntityColumnListMetadataModels { get; set; }

        public DbSet<LogParent> logParents { get; set; }

        public DbSet<LogChild> logChilds { get; set; }
        public DbSet<UserRoleModel> UserRoleModel { get; set; }

        public DbSet<UserTableModel> UserTableModel { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityColumnListMetadataModel>()
                .HasOne(e => e.EntityList)
                .WithMany(l => l.EntityColumns)
                .HasForeignKey(e => e.EntityId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
    }
}