using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.JoinedSubject
{
    public class ImportJoinedSubjectsForOneStudentRequestValidator 
        : AbstractValidator<ImportJoinedSubjectsForOneStudentRequest>
    {
        public ImportJoinedSubjectsForOneStudentRequestValidator()
        {
            // StudentUserName required
            RuleFor(x => x.StudentUserName)
                .NotEmpty().WithMessage("StudentUserName is required.");

            // Subjects collection required and not empty
            RuleFor(x => x.SubjectsData)
                .NotNull().WithMessage("Subjects collection cannot be null.")
                .Must(s => s.Count > 0).WithMessage("At least one subject is required.");

            // Apply the single-item validator to each subject in the set
            RuleForEach(x => x.SubjectsData)
                .SetValidator(new ImportJoinedSubjectsForOneStudent_DataValidator());
        }
    }

    // Validator for each subject item
    public class ImportJoinedSubjectsForOneStudent_DataValidator 
        : AbstractValidator<ImportJoinedSubjects_Data>
    {
        public ImportJoinedSubjectsForOneStudent_DataValidator()
        {
            // SubjectCode
            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("SubjectCode is required.");

            // SubjectVersionCode
            RuleFor(x => x.SubjectVersionCode)
                .NotEmpty().WithMessage("SubjectVersionCode is required.");

            // SemesterName with same format rule as SingleImportJoinedSubjectRequest
            RuleFor(x => x.SemesterName)
                .NotEmpty().WithMessage("SemesterName is required.")
                .Must(BeValidSemesterNameFormat)
                .WithMessage("SemesterName must be in format 'SpringYYYY', 'SummerYYYY', or 'FallYYYY'.");
        }

        private bool BeValidSemesterNameFormat(string semesterName)
        {
            if (string.IsNullOrWhiteSpace(semesterName))
                return false;

            var match = System.Text.RegularExpressions.Regex.Match(semesterName, @"^(Spring|Summer|Fall)(\d{4})$");

            if (!match.Success)
                return false;

            var yearStr = match.Groups[2].Value;
            if (int.TryParse(yearStr, out int year))
            {
                return year >= 2000 && year <= 2999;
            }

            return false;
        }
    }
}
