
using AISEA.ApiService.SHARED.DTOs.Requests.JoinedSubject;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.JoinedSubject
{
    public class ImportJoinedSubjectsRequestValidator
       : AbstractValidator<ImportJoinedSubjectsRequest>
    {
        public ImportJoinedSubjectsRequestValidator()
        {
            RuleFor(x => x.UserNameToSubjectsMap)
                .NotNull().WithMessage("UserNameToSubjectsMap cannot be null.")
                .Must(m => m.Count > 0).WithMessage("At least one student must be provided.");

            // Validate each entry in the dictionary
            RuleForEach(x => x.UserNameToSubjectsMap)
                .Must(entry => !string.IsNullOrWhiteSpace(entry.Key))
                    .WithMessage("Student username cannot be empty.")
                .DependentRules(() =>
                {
                    RuleForEach(x => x.UserNameToSubjectsMap)
                        .ChildRules(dict =>
                        {
                            // Reuse existing per-student validator logic
                            dict.RuleFor(x => x.Value)
                                .NotNull().WithMessage("Subjects collection cannot be null.")
                                .Must(s => s.Count > 0).WithMessage("At least one subject is required.")
                                .ForEach(subject =>
                                {
                                    subject.SetValidator(new ImportJoinedSubjectsForOneStudent_DataValidator());
                                });
                        });
                });
        }
    }
}