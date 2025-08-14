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

            // SemesterId must be positive
            RuleFor(x => x.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be a positive number.");

            //Semester Study Block Type
            RuleFor(x => x.SemesterStudyBlockType)
            .IsInEnum().WithMessage("SemesterStudyBlockType must be a valid enum value.");
        }
    }
}
