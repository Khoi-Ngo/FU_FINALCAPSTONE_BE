using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using FluentValidation;
using System.Text.RegularExpressions;

namespace AISEA.ApiService.BAL.Validators.JoinedSubject
{
    ///NOTE: Validation rules for this behavior
    //// No fields nullable or empty
    //// SubjectCode, SubjectVersionCode have to be existed in system (Trigger)
    //// The student must complete the pre-requisite courses of inserted course (Trigger)
    //// Semester name must have format ()
    //// SemesterName must be existed in system as SemesterName (Trigger)
    //// StudentUserName must be existed and have student profile when inserting course -> Auto fail when insert database due to foreign key constraint
    //// SubjectCode && VersionCode must be associated with the ComboName and CurriculumCode and ProgramCode of student (Trigger)
    //// The student must not be graduated (Trigger)
    //// The account of student must be active (Auto fail when query to get needed data before inserting)

    public class SingleImportJoinedSubjectRequestValidator : AbstractValidator<SingleImportJoinedSubjectRequest>
    {
        private static readonly string[] ValidSemesterPrefixes = { "Spring", "Summer", "Fall" };

        public SingleImportJoinedSubjectRequestValidator()
        {
            // StudentUserName
            RuleFor(x => x.StudentUserName)
                .NotEmpty().WithMessage("StudentUserName is required.");

            // SubjectCode
            RuleFor(x => x.SubjectCode)
                .NotEmpty().WithMessage("SubjectCode is required.");

            // SubjectVersionCode
            RuleFor(x => x.SubjectVersionCode)
                .NotEmpty().WithMessage("SubjectVersionCode is required.");

            // SemesterId must be positive
            RuleFor(x => x.SemesterId)
                .GreaterThan(0).WithMessage("SemesterId must be a positive number.");

            // SubjectName
            RuleFor(x => x.SubjectName)
                .NotEmpty().WithMessage("SubjectName is required.");
            //Semester Study Block Type
            RuleFor(x => x.SemesterStudyBlockType)
            .IsInEnum().WithMessage("SemesterStudyBlockType must be a valid enum value.");
        }

        private bool BeValidSemesterNameFormat(string semesterName)
        {
            if (string.IsNullOrWhiteSpace(semesterName))
                return false;

            // Regex pattern: (Spring|Summer|Fall) followed by 4-digit year
            var match = Regex.Match(semesterName, @"^(Spring|Summer|Fall)(\d{4})$");

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
