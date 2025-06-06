﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VehicleService.Infrastructure.Data;

#nullable disable

namespace VehicleService.Infrastructure.Migrations
{
    [DbContext(typeof(VehicleDbContext))]
    partial class VehicleDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VehicleService.Domain.Models.FuelRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<string>("DriverName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<float>("OdometerReading")
                        .HasColumnType("real");

                    b.Property<string>("Station")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("FuelRecords");
                });

            modelBuilder.Entity("VehicleService.Domain.Models.MaintenanceRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CompletedDate")
                        .HasColumnType("datetime2");

                    b.Property<float>("Cost")
                        .HasColumnType("real");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<float>("OdometerReading")
                        .HasColumnType("real");

                    b.Property<string>("Technician")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("VehicleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("VehicleId");

                    b.ToTable("MaintenanceRecords");
                });

            modelBuilder.Entity("VehicleService.Domain.Models.Vehicle", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssignedDriverId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<float>("CurrentFuelLevel")
                        .HasColumnType("real");

                    b.Property<float>("FuelCapacity")
                        .HasColumnType("real");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Model")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<float>("OdometerReading")
                        .HasColumnType("real");

                    b.Property<string>("RegistrationNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("VIN")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Vehicles");
                });

            modelBuilder.Entity("VehicleService.Domain.Models.FuelRecord", b =>
                {
                    b.HasOne("VehicleService.Domain.Models.Vehicle", null)
                        .WithMany("FuelHistory")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VehicleService.Domain.Models.MaintenanceRecord", b =>
                {
                    b.HasOne("VehicleService.Domain.Models.Vehicle", null)
                        .WithMany("MaintenanceHistory")
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("VehicleService.Domain.Models.Vehicle", b =>
                {
                    b.OwnsOne("VehicleService.Domain.Models.VehicleLocation", "LastKnownLocation", b1 =>
                        {
                            b1.Property<Guid>("VehicleId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<double>("Heading")
                                .HasColumnType("float")
                                .HasColumnName("LastHeading");

                            b1.Property<double>("Latitude")
                                .HasColumnType("float")
                                .HasColumnName("LastLatitude");

                            b1.Property<double>("Longitude")
                                .HasColumnType("float")
                                .HasColumnName("LastLongitude");

                            b1.Property<double>("Speed")
                                .HasColumnType("float")
                                .HasColumnName("LastSpeed");

                            b1.Property<DateTime>("Timestamp")
                                .HasColumnType("datetime2")
                                .HasColumnName("LastLocationTimestamp");

                            b1.HasKey("VehicleId");

                            b1.ToTable("Vehicles");

                            b1.WithOwner()
                                .HasForeignKey("VehicleId");
                        });

                    b.Navigation("LastKnownLocation")
                        .IsRequired();
                });

            modelBuilder.Entity("VehicleService.Domain.Models.Vehicle", b =>
                {
                    b.Navigation("FuelHistory");

                    b.Navigation("MaintenanceHistory");
                });
#pragma warning restore 612, 618
        }
    }
}
