using BusinessObjects.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessObjects
{
    public partial class ClassFileSharingContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ClassFileSharingContext()
        {
        }

        public ClassFileSharingContext(DbContextOptions<ClassFileSharingContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("APDBConnectionStr"));
        }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Resource> Resourses { get; set; } = null!;
        public virtual DbSet<BusinessObjects.Entities.File> Files { get; set; } = null!;
        public virtual DbSet<StudentCourse> StudentCourses { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Bỏ tiền tố AspNet của các bảng: mặc định các bảng trong IdentityDbContext có
            // tên với tiền tố AspNet như: AspNetUserRoles, AspNetUser ...
            // Đoạn mã sau chạy khi khởi tạo DbContext, tạo database sẽ loại bỏ tiền tố đó
            //foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            //{
            //    var tableName = entityType.GetTableName();
            //    if (tableName.StartsWith("AspNet"))
            //    {
            //        entityType.SetTableName(tableName.Substring(6));
            //    }
            //}
  
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasIndex(e => e.CourseName, "CourseName");
                

                entity.Property(e => e.CourseId).HasColumnName("CourseId");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}