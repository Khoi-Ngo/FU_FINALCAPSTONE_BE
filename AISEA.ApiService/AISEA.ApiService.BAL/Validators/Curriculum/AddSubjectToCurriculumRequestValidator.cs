using AISEA.ApiService.SHARED.DTOs.Requests.Curriculum;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Curriculum
{
    public class AddSubjectToCurriculumRequestValidator : AbstractValidator<AddSubjectToCurriculumRequest>
    {
        public AddSubjectToCurriculumRequestValidator()
        {
            RuleFor(x => x.SubjectVersionId)
                .GreaterThan(0).WithMessage("Subject Version ID must be greater than 0.");

            RuleFor(x => x.SemesterNumber)
                .GreaterThan(0).WithMessage("Semester number must be greater than 0.")
                .LessThanOrEqualTo(10).WithMessage("Semester number must be less than or equal to 10.");
        }
    }
}