using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCRM.Shared.Models.Activities;
using MyCRM.Shared.Models.Appointments;
using MyCRM.Shared.Models.Contacts;
using MyCRM.Shared.Models.Events;
using MyCRM.Shared.Models.Managements;
using MyCRM.Shared.Models.Pipelines;
using MyCRM.Shared.Models.Stages;
using MyCRM.Shared.Models.TargetTemplate;
using MyCRM.Shared.Models.Tasks;
using MyCRM.Shared.Models.User;

namespace MyCRM.Persistence.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //public string CurrentUserId { get; set; }
        public DbSet<Organization> Organizations { get; set; }

        public DbSet<People> Peoples { get; set; }
        public DbSet<Pipeline> Pipelines { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<TargetTemplate> TargetTemplates { get; set; }
        public DbSet<Activity> Activities { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Models.Roles.Employee, NormalizedName = Models.Roles.Employee.ToUpper() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Models.Roles.Supervisor, NormalizedName = Models.Roles.Supervisor.ToUpper() });
            //builder.Entity<IdentityRole>().HasData(new IdentityRole { Name = Models.Roles.SuperAdmin, NormalizedName = Models.Roles.SuperAdmin.ToUpper() });

            builder.Entity<Pipeline>().HasOne(x => x.ApplicationUser).WithMany(x => x.PipeLineFlows)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>().HasOne(s => s.Activity).WithMany(s => s.Appointments)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Task>().HasOne(s => s.Activity).WithMany(s => s.Tasks)
                .OnDelete(DeleteBehavior.SetNull);

            //builder.Entity<Pipeline>().HasOne(x => x.Appointment).WithMany(x => x.Pipelines)
            //   .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<People>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<Pipeline>().HasQueryFilter(s => !s.IsDeleted);
            //builder.Entity<Stage>().HasQueryFilter(s => !s.IsDeleted);
            builder.Entity<Activity>().HasIndex(x => x.Name);
            builder.Entity<Stage>().HasIndex(x => x.Name);
            builder.Entity<Pipeline>().HasIndex(x => x.DealAmount);
            builder.Entity<Pipeline>().HasIndex(x => x.DealName);
            builder.Entity<Pipeline>().HasIndex(x => x.ChangeStageDate);
            builder.Entity<Pipeline>().HasIndex(x => x.CreatedDate);
            builder.Entity<Pipeline>().HasIndex(x => x.Type);
            builder.Entity<Pipeline>().HasIndex(x => x.UpdatedDate);
            builder.Entity<Task>().HasIndex(x => x.EventStartDateTime);
            builder.Entity<Event>().HasIndex(x => x.EventStartDateTime);
            builder.Entity<Appointment>().HasIndex(x => x.EventStartDateTime);
            builder.Entity<People>().HasIndex(x => x.FirstName);
            builder.Entity<People>().HasIndex(x => x.LastName);
            builder.Entity<Company>().HasIndex(x => x.Name);

            //builder.Entity<Appointment>().HasOne(s => s.Pipeline).WithOne(s => s.Appointment)
            //    .HasForeignKey<Pipeline>(s => s.AppointmentId);

            //builder.Entity<Company>().HasQueryFilter(s => !s.IsDeleted);

            //builder.Entity<Employee>().HasQueryFilter(s => !s.IsDeleted);
            //builder.Entity<Organization>().HasQueryFilter(s => !s.IsDeleted);
            //builder.Entity<Stage>().HasQueryFilter(s => !s.IsDeleted);

            //builder.Entity<Pipeline>().OwnsOne(x=>x.Activity).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ApplicationUser>().HasMany(u => u.Claims).WithOne().HasForeignKey(c => c.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationUser>().HasMany(u => u.Roles).WithOne().HasForeignKey(r => r.UserId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Claims).WithOne().HasForeignKey(c => c.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<ApplicationRole>().HasMany(r => r.Users).WithOne().HasForeignKey(r => r.RoleId).IsRequired().OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Pipeline>().HasOne(s => s.Stage).WithMany(x => x.Pipelines)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Stage>().HasOne(s => s.Organization).WithMany(x => x.Stages)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Organization>().HasMany(s => s.Stages).WithOne(s => s.Organization).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Organization>().HasMany(s => s.Activities).WithOne(s => s.Organization).OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<Activity>().HasOne(x => x.Company).WithMany(x => x.Activities)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<Company>().HasMany(s => s.Activities).WithOne(s => s.Company)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}