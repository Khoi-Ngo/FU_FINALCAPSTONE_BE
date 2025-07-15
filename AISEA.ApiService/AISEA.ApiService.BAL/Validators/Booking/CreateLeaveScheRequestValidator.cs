using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Booking;

public class CreateLeaveScheRequestValidator : AbstractValidator<CreateLeaveScheRequest>
{
    public CreateLeaveScheRequestValidator()
    {
        RuleFor(x => x.StartDateTime)
            .NotNull().WithMessage("StartDateTime is required.")
            .Must(BeNotInPast).WithMessage("StartDateTime cannot be in the past.");

        RuleFor(x => x.EndDateTime)
            .NotNull().WithMessage("EndDateTime is required.");

        RuleFor(x => x)
            .Must(x => RoundToMinute(x.EndDateTime) > RoundToMinute(x.StartDateTime))
            .WithMessage("EndDateTime must be after StartDateTime after rounding to the nearest minute.");
    }

    private bool BeNotInPast(DateTime time)
    {
        return RoundToMinute(time) >= RoundToMinute(DateTime.UtcNow);
    }

    private DateTime RoundToMinute(DateTime time)
    {
        return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, DateTimeKind.Utc);
    }
}
