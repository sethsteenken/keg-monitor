﻿// <auto-generated />
using System;
using KegMonitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KegMonitor.Infrastructure.EntityFramework.Migrations
{
    [DbContext(typeof(KegMonitorDbContext))]
    partial class KegMonitorDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("KegMonitor.Core.Entities.Beer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ABV")
                        .HasColumnType("numeric")
                        .HasColumnName("abv");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<decimal>("FG")
                        .HasColumnType("numeric")
                        .HasColumnName("fg");

                    b.Property<string>("ImagePath")
                        .HasColumnType("text")
                        .HasColumnName("image_path");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("name");

                    b.Property<decimal>("OG")
                        .HasColumnType("numeric")
                        .HasColumnName("og");

                    b.Property<DateTime?>("TapDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("tap_date");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("type");

                    b.HasKey("Id");

                    b.ToTable("beers");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.BeerPour", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("BeerId")
                        .HasColumnType("integer")
                        .HasColumnName("beer_id");

                    b.Property<int>("ScaleId")
                        .HasColumnType("integer")
                        .HasColumnName("scale_id");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_stamp");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.HasIndex("ScaleId");

                    b.ToTable("beer_pours");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.Scale", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    b.Property<bool>("Active")
                        .HasColumnType("boolean")
                        .HasColumnName("active");

                    b.Property<int?>("BeerId")
                        .HasColumnType("integer")
                        .HasColumnName("beer_id");

                    b.Property<int>("CurrentWeight")
                        .HasColumnType("integer")
                        .HasColumnName("current_weight");

                    b.Property<int>("EmptyWeight")
                        .HasColumnType("integer")
                        .HasColumnName("empty_weight");

                    b.Property<string>("Endpoint")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("endpoint");

                    b.Property<int>("FullWeight")
                        .HasColumnType("integer")
                        .HasColumnName("full_weight");

                    b.Property<DateTime?>("LastDisabledDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_disabled_date");

                    b.Property<DateTime?>("LastEnabledDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_enabled_date");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_updated_date");

                    b.Property<int>("PourDifferenceThreshold")
                        .HasColumnType("integer")
                        .HasColumnName("pour_difference_threshold");

                    b.Property<string>("Topic")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("topic");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.ToTable("scales");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.ScaleWeightChange", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("BeerId")
                        .HasColumnType("integer")
                        .HasColumnName("beer_id");

                    b.Property<bool>("IsPourEvent")
                        .HasColumnType("boolean")
                        .HasColumnName("is_pour_event");

                    b.Property<int>("ScaleId")
                        .HasColumnType("integer")
                        .HasColumnName("scale_id");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("time_stamp");

                    b.Property<int>("Weight")
                        .HasColumnType("integer")
                        .HasColumnName("weight");

                    b.HasKey("Id");

                    b.HasIndex("BeerId");

                    b.HasIndex("ScaleId");

                    b.ToTable("scale_weight_changes");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.BeerPour", b =>
                {
                    b.HasOne("KegMonitor.Core.Entities.Beer", "Beer")
                        .WithMany("Pours")
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("KegMonitor.Core.Entities.Scale", "Scale")
                        .WithMany()
                        .HasForeignKey("ScaleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Beer");

                    b.Navigation("Scale");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.Scale", b =>
                {
                    b.HasOne("KegMonitor.Core.Entities.Beer", "Beer")
                        .WithMany()
                        .HasForeignKey("BeerId");

                    b.Navigation("Beer");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.ScaleWeightChange", b =>
                {
                    b.HasOne("KegMonitor.Core.Entities.Beer", "Beer")
                        .WithMany()
                        .HasForeignKey("BeerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("KegMonitor.Core.Entities.Scale", "Scale")
                        .WithMany("WeightChanges")
                        .HasForeignKey("ScaleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Beer");

                    b.Navigation("Scale");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.Beer", b =>
                {
                    b.Navigation("Pours");
                });

            modelBuilder.Entity("KegMonitor.Core.Entities.Scale", b =>
                {
                    b.Navigation("WeightChanges");
                });
#pragma warning restore 612, 618
        }
    }
}
