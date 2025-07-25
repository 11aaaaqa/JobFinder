﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using ResumeMicroservice.Api.Database;

#nullable disable

namespace ResumeMicroservice.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250502115120_remove_education_form_and_currently_work_here_columns")]
    partial class remove_education_form_and_currently_work_here_columns
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Resume", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AboutMe")
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<DateOnly?>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<long?>("DesiredSalary")
                        .HasColumnType("bigint");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<Guid>("EmployeeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Gender")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.PrimitiveCollection<List<string>>("OccupationTypes")
                        .HasColumnType("text[]");

                    b.Property<string>("Patronymic")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("ReadyToMove")
                        .HasColumnType("boolean");

                    b.Property<string>("ResumeTitle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.PrimitiveCollection<List<string>>("WorkTypes")
                        .HasColumnType("text[]");

                    b.Property<TimeSpan>("WorkingExperience")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.ToTable("Resumes");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.Education", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("EducationType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EducationalInstitution")
                        .HasColumnType("text");

                    b.Property<Guid?>("ResumeId")
                        .HasColumnType("uuid");

                    b.Property<string>("Specialization")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ResumeId");

                    b.ToTable("Education");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.EmployeeExperience", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CompanyPost")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Responsibilities")
                        .HasColumnType("text");

                    b.Property<Guid?>("ResumeId")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("WorkingDuration")
                        .HasColumnType("interval");

                    b.Property<string>("WorkingFrom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WorkingUntil")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ResumeId");

                    b.ToTable("EmployeeExperience");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.ForeignLanguage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("LanguageName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LanguageProficiencyLevel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ResumeId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ResumeId");

                    b.ToTable("ForeignLanguage");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.Education", b =>
                {
                    b.HasOne("ResumeMicroservice.Api.Models.Resume", null)
                        .WithMany("Educations")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.EmployeeExperience", b =>
                {
                    b.HasOne("ResumeMicroservice.Api.Models.Resume", null)
                        .WithMany("EmployeeExperience")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Skills.ForeignLanguage", b =>
                {
                    b.HasOne("ResumeMicroservice.Api.Models.Resume", null)
                        .WithMany("ForeignLanguages")
                        .HasForeignKey("ResumeId");
                });

            modelBuilder.Entity("ResumeMicroservice.Api.Models.Resume", b =>
                {
                    b.Navigation("Educations");

                    b.Navigation("EmployeeExperience");

                    b.Navigation("ForeignLanguages");
                });
#pragma warning restore 612, 618
        }
    }
}
