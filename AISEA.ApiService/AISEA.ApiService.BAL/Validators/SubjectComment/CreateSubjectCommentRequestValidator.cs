using AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.SubjectComment
{
    public class CreateSubjectCommentRequestValidator : AbstractValidator<CreateSubjectCommentRequest>
    {
        public CreateSubjectCommentRequestValidator()
        {
            RuleFor(x => x.SubjectId)
                .GreaterThan(0)
                .WithMessage("Subject ID must be greater than 0");

            RuleFor(x => x.Content)
                .NotEmpty()
                .WithMessage("Content is required")
                .Length(10, 2000)
                .WithMessage("Content must be between 10 and 2000 characters")
                .Must(BeValidContent)
                .WithMessage("Content contains inappropriate language");
        }

        private bool BeValidContent(string content)
        {
            // Basic content validation - can be enhanced with AI moderation
            if (string.IsNullOrWhiteSpace(content))
                return false;

            // Check for basic inappropriate patterns
            var inappropriateWords = new[] { "spam", "fake", "scam" };
            return !inappropriateWords.Any(word => 
                content.Contains(word, StringComparison.OrdinalIgnoreCase));
        }
    }
}
