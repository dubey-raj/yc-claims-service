using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClaimService.DataStorage.DAO;

public partial class YcEclaimsDbContext : DbContext
{
    public YcEclaimsDbContext()
    {
    }

    public YcEclaimsDbContext(DbContextOptions<YcEclaimsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<ClaimAssessment> ClaimAssessments { get; set; }

    public virtual DbSet<ClaimDocument> ClaimDocuments { get; set; }

    public virtual DbSet<Policy> Policies { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=yc-eclaims-db;Username=postgres;Password=admin");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Claim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("claims_pkey");

            entity.ToTable("claims");

            entity.HasIndex(e => e.ClaimNumber, "claims_claim_number_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ClaimNumber)
                .HasMaxLength(50)
                .HasColumnName("claim_number");
            entity.Property(e => e.IncidentDate).HasColumnName("incident_date");
            entity.Property(e => e.IncidentDescription).HasColumnName("incident_description");
            entity.Property(e => e.IncidentLocation)
                .HasMaxLength(255)
                .HasColumnName("incident_location");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_updated");
            entity.Property(e => e.PolicyId).HasColumnName("policy_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("submitted_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Policy).WithMany(p => p.Claims)
                .HasForeignKey(d => d.PolicyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("claims_policy_id_fkey");
        });

        modelBuilder.Entity<ClaimAssessment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("claim_assessments_pkey1");

            entity.ToTable("claim_assessments");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ClaimId).HasColumnName("claim_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DamageAssessed).HasColumnName("damage_assessed");
            entity.Property(e => e.EstimatedAmount)
                .HasPrecision(10, 2)
                .HasColumnName("estimated_amount");
            entity.Property(e => e.ReviewNote).HasColumnName("review_note");
            entity.Property(e => e.InspectionDate).HasColumnName("inspection_date");
            entity.Property(e => e.InspectorId).HasColumnName("inspector_id");
            entity.Property(e => e.ManagerId).HasColumnName("manager_id");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimAssessments)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("claim_assessments_claim_id_fkey");
        });

        modelBuilder.Entity<ClaimDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("claim_documents_pkey");

            entity.ToTable("claim_documents");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.ClaimId).HasColumnName("claim_id");
            entity.Property(e => e.FileType)
                .HasMaxLength(20)
                .HasColumnName("file_type");
            entity.Property(e => e.FileUrl).HasColumnName("file_url");
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("uploaded_at");

            entity.HasOne(d => d.Claim).WithMany(p => p.ClaimDocuments)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("claim_documents_claim_id_fkey");
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("policies_pkey");

            entity.ToTable("policies");

            entity.HasIndex(e => e.PolicyNumber, "policies_policy_number_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CoverageAmount)
                .HasPrecision(12, 2)
                .HasColumnName("coverage_amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.DeductibleAmount)
                .HasPrecision(10, 2)
                .HasColumnName("deductible_amount");
            entity.Property(e => e.EffectiveDate).HasColumnName("effective_date");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(e => e.InsurerName)
                .HasMaxLength(100)
                .HasColumnName("insurer_name");
            entity.Property(e => e.PolicyNumber)
                .HasMaxLength(50)
                .HasColumnName("policy_number");
            entity.Property(e => e.PolicyType)
                .HasMaxLength(50)
                .HasColumnName("policy_type");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("vehicles_pkey");

            entity.ToTable("vehicles");

            entity.HasIndex(e => e.RegistrationNumber, "vehicles_registration_number_key").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Make)
                .HasMaxLength(50)
                .HasColumnName("make");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.RegistrationNumber)
                .HasMaxLength(20)
                .HasColumnName("registration_number");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasMany(d => d.Policies).WithMany(p => p.Vehicles)
                .UsingEntity<Dictionary<string, object>>(
                    "VehiclePolicy",
                    r => r.HasOne<Policy>().WithMany()
                        .HasForeignKey("PolicyId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("vehicle_policy_policy_id_fkey"),
                    l => l.HasOne<Vehicle>().WithMany()
                        .HasForeignKey("VehicleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("vehicle_policy_vehicle_id_fkey"),
                    j =>
                    {
                        j.HasKey("VehicleId", "PolicyId").HasName("vehicle_policy_pkey");
                        j.ToTable("vehicle_policy");
                        j.IndexerProperty<long>("VehicleId").HasColumnName("vehicle_id");
                        j.IndexerProperty<long>("PolicyId").HasColumnName("policy_id");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
