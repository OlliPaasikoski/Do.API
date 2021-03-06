﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Do.API.Entities;

namespace Do.API.Migrations
{
    [DbContext(typeof(DoContext))]
    [Migration("20170609110525_BlogsAndTasksJoinTable")]
    partial class BlogsAndTasksJoinTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Do.API.Entities.BlogPost", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Content")
                        .HasMaxLength(500);

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("BlogPosts");
                });

            modelBuilder.Entity("Do.API.Entities.BlogPostTask", b =>
                {
                    b.Property<Guid>("BlogPostId");

                    b.Property<Guid>("TaskId");

                    b.HasKey("BlogPostId", "TaskId");

                    b.HasIndex("TaskId");

                    b.ToTable("BlogPostTasks");
                });

            modelBuilder.Entity("Do.API.Entities.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CategoryId");

                    b.Property<DateTimeOffset>("Date");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500);

                    b.Property<bool>("Success");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Do.API.Entities.TaskCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(500);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("TaskCategories");
                });

            modelBuilder.Entity("Do.API.Entities.BlogPostTask", b =>
                {
                    b.HasOne("Do.API.Entities.BlogPost", "BlogPost")
                        .WithMany("RelatedTasks")
                        .HasForeignKey("BlogPostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Do.API.Entities.Task", "Task")
                        .WithMany("RelatedBlogs")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Do.API.Entities.Task", b =>
                {
                    b.HasOne("Do.API.Entities.TaskCategory", "Category")
                        .WithMany("Tasks")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
