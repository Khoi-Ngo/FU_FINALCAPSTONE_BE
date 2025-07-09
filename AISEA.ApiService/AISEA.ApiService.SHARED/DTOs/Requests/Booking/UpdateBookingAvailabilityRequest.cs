using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Booking
{
    public class UpdateBookingAvailabilityRequest
    {
        public required TimeSpan StartTime { get; set; }

        public required TimeSpan EndTime { get; set; }

        public required DayOfWeek DayInWeek { get; set; }

    }
}