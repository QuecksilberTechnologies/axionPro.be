using axionpro.domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace axionpro.persistance.Data.Context;

public partial class WorkforceDbContext
{
    private static void ConfigureGenericPolicyFramework(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PolicyCategory>(entity =>
        {
            entity.ToTable("PolicyCategory", "axionpro");
            entity.Property(e => e.CategoryCode).HasMaxLength(50);
            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<PolicyStatus>(entity =>
        {
            entity.ToTable("PolicyStatus", "axionpro");
            entity.Property(e => e.StatusCode).HasMaxLength(30);
            entity.Property(e => e.StatusName).HasMaxLength(50);
        });

        modelBuilder.Entity<PolicyRuleType>(entity =>
        {
            entity.ToTable("PolicyRuleType", "axionpro");
            entity.Property(e => e.RuleTypeCode).HasMaxLength(50);
            entity.Property(e => e.RuleTypeName).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<PolicyDocumentType>(entity =>
        {
            entity.ToTable("PolicyDocumentType", "axionpro");
            entity.Property(e => e.DocumentTypeCode).HasMaxLength(30);
            entity.Property(e => e.DocumentTypeName).HasMaxLength(100);
        });

        modelBuilder.Entity<PolicyType>(entity =>
        {
            entity.ToTable("PolicyType", "axionpro");
            entity.Property(e => e.PolicyName).HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PolicyTypeCode).HasMaxLength(50);
            entity.Property(e => e.DefaultCurrencyCode).HasMaxLength(3);
        });

        modelBuilder.Entity<Policy>(entity =>
        {
            entity.ToTable("Policy", "axionpro");
            entity.Property(e => e.PolicyCode).HasMaxLength(50);
            entity.Property(e => e.PolicyName).HasMaxLength(200);
            entity.Property(e => e.Summary).HasMaxLength(1000);
            entity.Property(e => e.DefaultCurrencyCode).HasMaxLength(3);
            entity.HasIndex(e => new { e.TenantId, e.PolicyCode }).IsUnique();
        });

        modelBuilder.Entity<PolicyVersion>(entity =>
        {
            entity.ToTable("PolicyVersion", "axionpro");
            entity.Property(e => e.EffectiveFrom).HasColumnType("date");
            entity.Property(e => e.EffectiveTo).HasColumnType("date");
            entity.Property(e => e.ChangeSummary).HasMaxLength(1000);
            entity.HasIndex(e => new { e.PolicyId, e.VersionNumber }).IsUnique();
            entity.HasOne(e => e.Policy).WithMany(e => e.PolicyVersions)
                .HasForeignKey(e => e.PolicyId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AttendancePolicyVersionConfiguration>(entity =>
        {
            entity.ToTable("AttendancePolicyVersionConfiguration", "axionpro");
            entity.HasIndex(e => e.PolicyVersionId).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.PolicyVersionId }).IsUnique();
            entity.HasOne(e => e.PolicyVersion).WithOne(e => e.AttendanceConfiguration)
                .HasForeignKey<AttendancePolicyVersionConfiguration>(e => e.PolicyVersionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PolicyRule>(entity =>
        {
            entity.ToTable("PolicyRule", "axionpro");
            entity.Property(e => e.RuleName).HasMaxLength(150);
            entity.Property(e => e.RuleConfiguration).HasColumnType("jsonb");
            entity.HasIndex(e => new { e.PolicyVersionId, e.RuleOrder }).IsUnique();
        });

        modelBuilder.Entity<PolicyApplicability>(entity =>
        {
            entity.ToTable("PolicyApplicability", "axionpro");
            entity.Property(e => e.EffectiveFrom).HasColumnType("date");
            entity.Property(e => e.EffectiveTo).HasColumnType("date");
        });

        modelBuilder.Entity<PolicyAssignment>(entity =>
        {
            entity.ToTable("PolicyAssignment", "axionpro");
            entity.Property(e => e.EffectiveFrom).HasColumnType("date");
            entity.Property(e => e.EffectiveTo).HasColumnType("date");
            entity.HasIndex(e => new { e.PolicyVersionId, e.EmployeeId, e.EffectiveFrom }).IsUnique();
        });

        modelBuilder.Entity<PolicyException>(entity =>
        {
            entity.ToTable("PolicyException", "axionpro");
            entity.Property(e => e.OverrideConfiguration).HasColumnType("jsonb");
            entity.Property(e => e.Reason).HasMaxLength(1000);
            entity.Property(e => e.EffectiveFrom).HasColumnType("date");
            entity.Property(e => e.EffectiveTo).HasColumnType("date");
        });

        modelBuilder.Entity<PolicyDocument>(entity =>
        {
            entity.ToTable("PolicyDocument", "axionpro");
            entity.Property(e => e.DocumentTitle).HasMaxLength(200);
            entity.Property(e => e.OriginalFileName).HasMaxLength(255);
            entity.Property(e => e.StorageProvider).HasMaxLength(30);
            entity.Property(e => e.ObjectKey).HasMaxLength(1000);
            entity.Property(e => e.ContentType).HasMaxLength(100);
            entity.Property(e => e.ChecksumSha256).HasMaxLength(64);
            entity.Property(e => e.LanguageCode).HasMaxLength(10);
        });

        modelBuilder.Entity<PolicyApprovalStage>(entity =>
        {
            entity.ToTable("PolicyApprovalStage", "axionpro");
            entity.Property(e => e.StageName).HasMaxLength(100);
            entity.HasIndex(e => new { e.TenantId, e.PolicyCategoryId, e.StageOrder }).IsUnique();
        });

        modelBuilder.Entity<PolicyApprovalHistory>(entity =>
        {
            entity.ToTable("PolicyApprovalHistory", "axionpro");
            entity.Property(e => e.Comments).HasMaxLength(1000);
            entity.HasIndex(e => new { e.PolicyVersionId, e.SequenceNumber }).IsUnique();
        });

        modelBuilder.Entity<PolicyAcknowledgement>(entity =>
        {
            entity.ToTable("PolicyAcknowledgement", "axionpro");
            entity.Property(e => e.SourceIpHash).HasMaxLength(64);
            entity.Property(e => e.EvidenceJson).HasColumnType("jsonb");
            entity.HasIndex(e => new { e.PolicyVersionId, e.EmployeeId }).IsUnique();
        });

        modelBuilder.Entity<PolicyChangeAudit>(entity =>
        {
            entity.ToTable("PolicyChangeAudit", "axionpro");
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.ActionName).HasMaxLength(50);
            entity.Property(e => e.BeforeData).HasColumnType("jsonb");
            entity.Property(e => e.AfterData).HasColumnType("jsonb");
        });
    }
}
