﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Reveche.SimpleLearnerInfoSystem.Console.Data;

#nullable disable

namespace Reveche.SimpleLearnerInfoSystem.Console.Migrations.SqliteMigrations
{
    [DbContext(typeof(SqliteDbContext))]
    partial class SqliteDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.1");

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.Course", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("DurationInHours")
                        .HasColumnType("INTEGER");

                    b.Property<int>("InstructorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Term")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProgramId");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.CourseCompletion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CourseId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<double?>("Grade")
                        .HasColumnType("REAL");

                    b.Property<int>("InstructorId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProgramTrackerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProgramTrackerId");

                    b.ToTable("CourseCompletions");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.Program", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Programs");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.ProgramProgress", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DateCompleted")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProgramId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ProgramTrackerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ProgramTrackerId");

                    b.ToTable("ProgramProgress");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.ProgramTracker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ProgramTrackers");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBool")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsInt")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsLong")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsString")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Scope")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(255)
                        .HasColumnType("INTEGER");

                    b.Property<string>("AddressBarangay")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressCity")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressCountryCode")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressProvince")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressStreet")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("AddressZipCode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("BirthDate")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<long>("PhoneNumber")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Role")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserIdStr")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("YearLevel")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.Course", b =>
                {
                    b.HasOne("Reveche.SimpleLearnerInfoSystem.Models.Program", null)
                        .WithMany("Courses")
                        .HasForeignKey("ProgramId");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.CourseCompletion", b =>
                {
                    b.HasOne("Reveche.SimpleLearnerInfoSystem.Models.ProgramTracker", null)
                        .WithMany("Courses")
                        .HasForeignKey("ProgramTrackerId");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.ProgramProgress", b =>
                {
                    b.HasOne("Reveche.SimpleLearnerInfoSystem.Models.ProgramTracker", null)
                        .WithMany("Programs")
                        .HasForeignKey("ProgramTrackerId");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.Program", b =>
                {
                    b.Navigation("Courses");
                });

            modelBuilder.Entity("Reveche.SimpleLearnerInfoSystem.Models.ProgramTracker", b =>
                {
                    b.Navigation("Courses");

                    b.Navigation("Programs");
                });
#pragma warning restore 612, 618
        }
    }
}