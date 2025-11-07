using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using MockApiServer.Data.Entities;

namespace MockApiServer.Data;

public partial class MockApiServerDbContext : DbContext
{
    public MockApiServerDbContext(DbContextOptions<MockApiServerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Mock> Mocks { get; set; }

    public virtual DbSet<MockScenario> MockScenarios { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("applications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
        });

        modelBuilder.Entity<Mock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mocks");

            entity.HasIndex(e => e.ApplicationId, "fk_mocks_applications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.Method)
                .HasMaxLength(20)
                .HasColumnName("method");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Path)
                .HasMaxLength(255)
                .HasColumnName("path");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Application).WithMany(p => p.Mocks)
                .HasForeignKey(d => d.ApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mocks_applications");
        });

        modelBuilder.Entity<MockScenario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("mock_scenarios");

            entity.HasIndex(e => e.MockId, "fk_mock_scenarios_mocks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.HeadersJson)
                .HasColumnType("text")
                .HasColumnName("headers_json");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.MockId).HasColumnName("mock_id");
            entity.Property(e => e.ResponseJson)
                .HasColumnType("text")
                .HasColumnName("response_json");
            entity.Property(e => e.ScenarioKey)
                .HasMaxLength(100)
                .HasColumnName("scenario_key");
            entity.Property(e => e.StatusCode).HasColumnName("status_code");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");

            entity.HasOne(d => d.Mock).WithMany(p => p.MockScenarios)
                .HasForeignKey(d => d.MockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_mock_scenarios_mocks");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .UseCollation("utf8mb4_unicode_ci");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100)
                .HasColumnName("created_by");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_active");
            entity.Property(e => e.LastLoginTime)
                .HasColumnType("datetime")
                .HasColumnName("last_login_time");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(255)
                .HasColumnName("mobile_no");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Remarks)
                .HasMaxLength(500)
                .HasColumnName("remarks");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100)
                .HasColumnName("updated_by");
            entity.Property(e => e.Username)
                .HasMaxLength(150)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
