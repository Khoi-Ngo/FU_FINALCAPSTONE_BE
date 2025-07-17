using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using FluentValidation;

namespace AISEA.ApiService.BAL.Validators.Booking;

public class CreateBookingAvailabilityRequestValidator : AbstractValidator<CreateBookingAvailabilityRequest>
{
    public CreateBookingAvailabilityRequestValidator()
    {
        RuleFor(x => x.StartTime)
            .Must(BeValidTimeSpan).WithMessage("StartTime must be a valid time between 00:00:00 and 23:59:59.")
            .NotNull().WithMessage("StartTime is required.");

        RuleFor(x => x.EndTime)
            .Must(BeValidTimeSpan).WithMessage("EndTime must be a valid time between 00:00:00 and 23:59:59.")
            .NotNull().WithMessage("EndTime is required.");

        RuleFor(x => x)
            .Must(x => RoundToMinute(x.EndTime) > RoundToMinute(x.StartTime))
            .WithMessage("EndTime must be after StartTime after rounding to the nearest minute.");

        RuleFor(x => x.DayInWeek)
            .IsInEnum().WithMessage("DayInWeek must be a valid day of the week (Monday to Sunday).");

        RuleFor(x => x)
            .Must(x =>
            {
                var duration = RoundToMinute(x.EndTime) - RoundToMinute(x.StartTime);
                return duration >= TimeSpan.FromMinutes(30) && duration <= TimeSpan.FromHours(2);
            })
            .WithMessage("The time range must be between 30 minutes and 2 hours.");

    }

    private bool BeValidTimeSpan(TimeSpan time)
    {
        return time >= TimeSpan.Zero && time < TimeSpan.FromDays(1);
    }

    private TimeSpan RoundToMinute(TimeSpan time)
    {
        return new TimeSpan(time.Hours, time.Minutes, 0);
    }
}