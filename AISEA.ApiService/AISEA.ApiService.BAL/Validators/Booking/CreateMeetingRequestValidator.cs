using AISEA.ApiService.SHARED.DTOs.Requests.Booking;
using AISEA.ApiService.SHARED.PropConfigs;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AISEA.ApiService.BAL.Validators.Booking;

public class CreateMeetingRequestValidator : AbstractValidator<CreateMeetingRequest>
{
    private readonly BookingSettings _bookingSettings;

    public CreateMeetingRequestValidator(BookingSettings bookingSettings)
    {
        _bookingSettings = bookingSettings;

        RuleFor(x => x.StaffProfileId)
            .GreaterThan(0).WithMessage("StaffProfileId must be a positive non-zero value.");

        RuleFor(x => x.StartDateTime)
            .NotNull().WithMessage("StartDateTime is required.")
            .Must(BeNotInPast).WithMessage("StartDateTime cannot be in the past.")
            .Must(BeAtLeastMinDaysAhead)
            .WithMessage($"StartDateTime must be at least {_bookingSettings.MinTimeToGoStuCreateMeetingDays} days from now to allow scheduling preparation.");

        RuleFor(x => x.EndDateTime)
            .NotNull().WithMessage("EndDateTime is required.");

        RuleFor(x => x)
            .Must(x => RoundToMinute(x.EndDateTime) > RoundToMinute(x.StartDateTime))
            .WithMessage("EndDateTime must be after StartDateTime after rounding to the nearest minute.");

        RuleFor(x => x)
            .Must(x => x.StartDateTime.Date == x.EndDateTime.Date)
            .WithMessage("Meeting must be wrapped within a single day.");

        RuleFor(x => x.TitleStudentIssue)
            .NotEmpty().WithMessage("TitleStudentIssue is required.")
            .MaximumLength(200).WithMessage("TitleStudentIssue cannot exceed 200 characters.");

        RuleFor(x => x.ContentIssue)
            .NotEmpty().WithMessage("ContentIssue is required.")
            .MaximumLength(1000).WithMessage("ContentIssue cannot exceed 1000 characters.");
    }

    private bool BeNotInPast(DateTime time)
    {
        return RoundToMinute(time) >= RoundToMinute(DateTime.UtcNow);
    }

    private bool BeAtLeastMinDaysAhead(DateTime time)
    {
        return RoundToMinute(time) >= RoundToMinute(DateTime.UtcNow.AddDays(_bookingSettings.MinTimeToGoStuCreateMeetingDays));
    }

    private DateTime RoundToMinute(DateTime time)
    {
        return new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0, DateTimeKind.Utc);
    }
}
