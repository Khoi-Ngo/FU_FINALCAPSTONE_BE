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
                .Must(NotContainObviousSpam)
                .WithMessage("Content contains potentially inappropriate language");
        }

        private bool NotContainObviousSpam(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            var lowerContent = content.ToLowerInvariant();

            // Strong profanity check - immediate rejection

            var strongProfanity = new[]
            {
                "fuck", "fucking", "shit", "bitch", "bastard", "asshole", "ass hole",
                "damn it", "goddamn", "god damn", "son of a bitch", "piece of shit"
            };

            if (strongProfanity.Any(word => lowerContent.Contains(word)))
                return false;


            // Basic spam/inappropriate patterns
            var obviousSpamPatterns = new[]
            {
                "click here", "visit our website", "buy now", "free money",
                "make money fast", "work from home", "easy cash",
                "www.", "http://", "https://", ".com", ".net", ".org",
                "essay writing service", "homework help service", "buy essay"
            };

            if (obviousSpamPatterns.Any(pattern => lowerContent.Contains(pattern)))
                return false;

            // Personal attacks
            var personalAttacks = new[]
            {
                "you are stupid", "you're stupid", "you suck", "shut up",
                "go kill yourself", "kill yourself", "you're worthless"
            };

            if (personalAttacks.Any(pattern => lowerContent.Contains(pattern)))
                return false;

            return true;
        }
    }
}
