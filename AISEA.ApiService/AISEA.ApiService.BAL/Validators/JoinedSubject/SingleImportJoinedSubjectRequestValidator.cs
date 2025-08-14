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

            //Semester Study Block Type
            RuleFor(x => x.SemesterStudyBlockType)
            .IsInEnum().WithMessage("SemesterStudyBlockType must be a valid enum value.");
        }

    }
}
