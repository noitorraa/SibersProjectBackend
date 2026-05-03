using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using sibersProject.Data.Entities;

namespace sibersProject.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectDocument> ProjectDocuments { get; set; }

    public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=Database/sibersDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_Companies_Name").IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasIndex(e => e.Email, "IX_Employees_Email").IsUnique();

            entity.HasIndex(e => e.Email, "idx_employees_email");

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "idx_employees_name");

            entity.Property(e => e.IsActive).HasDefaultValue(1);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasIndex(e => e.Priority, "idx_projects_priority");

            entity.HasIndex(e => e.StartDate, "idx_projects_startdate");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValue("Active");

            entity.HasOne(d => d.ContractorCompany).WithMany(p => p.ProjectContractorCompanies)
                .HasForeignKey(d => d.ContractorCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.CustomerCompany).WithMany(p => p.ProjectCustomerCompanies)
                .HasForeignKey(d => d.CustomerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Manager).WithMany(p => p.Projects)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ProjectDocument>(entity =>
        {
            entity.Property(e => e.UploadedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectDocuments).HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<ProjectEmployee>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.EmployeeId });

            entity.HasIndex(e => e.EmployeeId, "idx_projectemployees_employee");

            entity.HasIndex(e => e.ProjectId, "idx_projectemployees_project");

            entity.HasOne(d => d.Employee).WithMany(p => p.ProjectEmployees).HasForeignKey(d => d.EmployeeId);

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectEmployees).HasForeignKey(d => d.ProjectId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
