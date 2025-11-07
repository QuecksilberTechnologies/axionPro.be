using System;
using System.Collections.Generic;

namespace axionpro.domain.Entity;

public partial class Candidate
{
    public long Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Pan { get; set; }

    public string? Aadhaar { get; set; }

    public string CandidateReferenceCode { get; set; } = null!;

    public bool IsBlacklisted { get; set; }

    public decimal ExperienceYears { get; set; }

    public string? CurrentLocation { get; set; }

    public decimal? ExpectedSalary { get; set; }

    public string? CurrentCompany { get; set; }

    public int? NoticePeriod { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateTime AppliedDate { get; set; }

    public DateTime? LastUpdatedDateTime { get; set; }

    public string? SkillSet { get; set; }

    public bool IsActive { get; set; }

    public string? ActionStatus { get; set; }

    public string? Education { get; set; }

    public bool? IsFresher { get; set; }

    public string? FewWords { get; set; }

    public string? ResumePath { get; set; }

    public string? ResumeUrl { get; set; }

    public virtual ICollection<CandidateCategorySkill> CandidateCategorySkills { get; set; } = new List<CandidateCategorySkill>();

    public virtual ICollection<CandidateHistory> CandidateHistories { get; set; } = new List<CandidateHistory>();

    public virtual ICollection<InterviewFeedback> InterviewFeedbacks { get; set; } = new List<InterviewFeedback>();

    public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; } = new List<InterviewSchedule>();
}
