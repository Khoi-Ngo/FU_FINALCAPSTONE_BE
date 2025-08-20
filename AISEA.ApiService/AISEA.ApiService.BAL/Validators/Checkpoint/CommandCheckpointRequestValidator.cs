using AISEA.ApiService.SHARED.DTOs.Requests.CheckPoint;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Checkpoint
{
    public class CommandCheckpointRequestValidator : AbstractValidator<CommandCheckpointRequest>
    {
        public CommandCheckpointRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.")
                .MaximumLength(2000).WithMessage("Content cannot exceed 2000 characters.");

            RuleFor(x => x.Deadline)
                .NotEmpty().WithMessage("Deadline is required.");
        }
    }
}
