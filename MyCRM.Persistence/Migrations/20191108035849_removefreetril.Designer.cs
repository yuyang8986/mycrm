﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyCRM.Persistence.Data;

namespace MyCRM.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20191108035849_removefreetril")]
    partial class removefreetril
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Activities.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ActivityType");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Activities");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Appointments.Appointment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ActivityId");

                    b.Property<string>("ApplicationUserId");

                    b.Property<DateTime?>("CompleteTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("DurationMinutes");

                    b.Property<DateTime>("EventStartDateTime");

                    b.Property<bool>("IsCompleted");

                    b.Property<bool>("IsReminderOn");

                    b.Property<string>("Location");

                    b.Property<string>("Note");

                    b.Property<Guid?>("PipelineId");

                    b.Property<string>("Summary")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("PipelineId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Contacts.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("Email");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Location");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("SecondaryEmail");

                    b.Property<string>("SecondaryPhone");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Contacts.People", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CompanyId");

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsCustomer");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("LastName");

                    b.Property<string>("Phone");

                    b.Property<string>("WorkEmail");

                    b.Property<string>("WorkPhone");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Peoples");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Events.Event", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ActivityId");

                    b.Property<int?>("CompanyId");

                    b.Property<DateTime?>("CompleteTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("DurationMinutes");

                    b.Property<DateTime>("EventStartDateTime");

                    b.Property<bool>("IsCompleted");

                    b.Property<bool>("IsReminderOn");

                    b.Property<string>("Location");

                    b.Property<string>("Note");

                    b.Property<int>("OrganizationId");

                    b.Property<string>("Summary")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Managements.Organization", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ReferralCode");

                    b.Property<string>("StripeCustomerId");

                    b.Property<string>("StripeSubscriptionId");

                    b.Property<DateTime?>("SubscriptionExpirationDate");

                    b.Property<int>("SubscriptionPlan");

                    b.Property<int>("SubscriptionQuantity");

                    b.Property<DateTime?>("SubscriptionStartDate");

                    b.HasKey("Id");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Pipelines.Pipeline", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ApplicationUserId");

                    b.Property<DateTime>("ChangeStageDate");

                    b.Property<int?>("CompanyId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<double>("DealAmount");

                    b.Property<string>("DealName");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsStarred");

                    b.Property<int?>("PeopleId");

                    b.Property<int>("StageId");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("CompanyId");

                    b.HasIndex("PeopleId");

                    b.HasIndex("StageId");

                    b.ToTable("Pipelines");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Stages.Stage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DisplayIndex");

                    b.Property<int?>("IconIndex");

                    b.Property<bool>("IsDeleted");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.TargetTemplate.TargetTemplate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsArchive");

                    b.Property<string>("Name");

                    b.Property<int>("OrganizationId");

                    b.Property<double>("Q1");

                    b.Property<double>("Q2");

                    b.Property<double>("Q3");

                    b.Property<double>("Q4");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("TargetTemplates");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Tasks.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("ActivityId");

                    b.Property<string>("ApplicationUserId");

                    b.Property<DateTime?>("CompleteTime");

                    b.Property<DateTime>("CreatedTime");

                    b.Property<int>("DurationMinutes");

                    b.Property<DateTime>("EventStartDateTime");

                    b.Property<bool>("IsCompleted");

                    b.Property<bool>("IsReminderOn");

                    b.Property<string>("Location");

                    b.Property<string>("Note");

                    b.Property<Guid?>("PipelineId");

                    b.Property<string>("Summary")
                        .HasMaxLength(30);

                    b.HasKey("Id");

                    b.HasIndex("ActivityId");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("PipelineId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.User.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.User.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<int>("OrganizationId");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<Guid?>("TargetTemplateId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.Property<string>("VerifyCode");

                    b.Property<DateTime>("VerifyExpiredDateTime");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("TargetTemplateId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationRole")
                        .WithMany("Claims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationRole")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Activities.Activity", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Managements.Organization", "Organization")
                        .WithMany("Activities")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Appointments.Appointment", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Activities.Activity", "Activity")
                        .WithMany("Appointments")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser", "ApplicationUser")
                        .WithMany("Appointments")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("MyCRM.Shared.Models.Pipelines.Pipeline", "Pipeline")
                        .WithMany("Appointments")
                        .HasForeignKey("PipelineId");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Contacts.Company", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser", "ApplicationUser")
                        .WithMany("Companies")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Contacts.People", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Contacts.Company", "Company")
                        .WithMany("Peoples")
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Events.Event", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Activities.Activity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyCRM.Shared.Models.Contacts.Company", "Company")
                        .WithMany("Events")
                        .HasForeignKey("CompanyId");

                    b.HasOne("MyCRM.Shared.Models.Managements.Organization", "Organization")
                        .WithMany("Events")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Pipelines.Pipeline", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser", "ApplicationUser")
                        .WithMany("PipeLineFlows")
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MyCRM.Shared.Models.Contacts.Company", "Company")
                        .WithMany("Pipelines")
                        .HasForeignKey("CompanyId");

                    b.HasOne("MyCRM.Shared.Models.Contacts.People", "People")
                        .WithMany("Pipelines")
                        .HasForeignKey("PeopleId");

                    b.HasOne("MyCRM.Shared.Models.Stages.Stage", "Stage")
                        .WithMany("Pipelines")
                        .HasForeignKey("StageId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Stages.Stage", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Managements.Organization", "Organization")
                        .WithMany("Stages")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.TargetTemplate.TargetTemplate", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Managements.Organization", "Organization")
                        .WithMany("TargetTemplates")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MyCRM.Shared.Models.Tasks.Task", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Activities.Activity", "Activity")
                        .WithMany("Tasks")
                        .HasForeignKey("ActivityId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MyCRM.Shared.Models.User.ApplicationUser", "ApplicationUser")
                        .WithMany("Tasks")
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("MyCRM.Shared.Models.Pipelines.Pipeline", "Pipeline")
                        .WithMany("Tasks")
                        .HasForeignKey("PipelineId");
                });

            modelBuilder.Entity("MyCRM.Shared.Models.User.ApplicationUser", b =>
                {
                    b.HasOne("MyCRM.Shared.Models.Managements.Organization", "Organization")
                        .WithMany("ApplicationUsers")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MyCRM.Shared.Models.TargetTemplate.TargetTemplate", "TargetTemplate")
                        .WithMany("Employees")
                        .HasForeignKey("TargetTemplateId");
                });
#pragma warning restore 612, 618
        }
    }
}
