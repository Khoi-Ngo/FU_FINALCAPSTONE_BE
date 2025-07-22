namespace AISEA.ApiService.SHARED.Const.Enums
{
    public enum EBookingStatus
    {
        PENDING = 1, //VALID TIME TO DO: FUTURE (before 2 days to go), VALID CUR STAT: NONE
        CONFIRMED = 2,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 1 day, VALID CUR STAT: PENDING
        ADV_CANCELED = 3,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 12hours, VALID CUR STAT: CONFIRMED
        /// <summary>
        /// VALID CUR STAT: CONFIRMED || PENDING
        /// case: PENDING then no restriction VALID TIME: Before the EndTime of the meeting timeSlot
        /// case: CONFIRMED: Before the StartTime of the meeting timeSlot < 12hours (!Not Allowed)
        /// case: CONFIRMED: Before the StartTime of the meeting timeSlot 3 days -> 12hours (!Having permission)
        /// case: CONFIRMED: Before the StartTime of the meeting timeSlot > 3days (NO having permission)
        /// </summary>
        STU_CANCELED = 9,
        COMPLETED = 4,//VALID TIME TO DO: after the EndTime of the meeting TimeSlot, VALID CUR STAT: CONFIRMED, VALID CONFIRM CHECK IN CODE
        /// <summary>
        /// VALID CUR STAT: CONFIRMED
        /// case: Advisor mark the student missing the meeting VALID TIME TO DO: >= EndTime of the meeting timeSlot
        /// case: Nothing happen when Current Time > EndTime of the meeting timeSlot + 1 day, then the worker service will handle Shift from CONFIRMED  to STUDENT_MISSED stat
        /// </summary>
        STUDENT_MISSED = 5,
        ADVISOR_MISSED = 6, //VALID TIME TO DO: After the 15 mins from StartDateTime of the meeting TimeSlot VALID CUR STAT: CONFIRMED
        NOT_APPROVED = 7,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 12hours, VALID CUR STAT: PENDING
        OVERDUE = 8,// After the EndTime of the meeting timeSlot the worker service will handle shift Stat from PENDING to OVERDUE

        /*

        ///The student can only book when there is no existed meeting in that time slot or STU_CANCELED only (Handle By Trigger Database)

        ///The advisor can only log leave when there is no meeting in that time slot or if already had then those should be only incase (NOT_APPROVE, ADV_CANCELED, STU_CANCEL)

        /// Other case need Advisor take action (Cancel them or do something similar like that ...) to be able to complete the logging leave (PENDING, CONFIRMED)

        /// Other case will never happen if handle right(because only allow the advisor logging leave for the Time in the future): Create Leave when there is meeting (OVERDUE, ADVISOR_MISSED, STUDENT_MISSED, COMPLETED, )

        */
    }

    public enum DayOfWeekAISEA
    {
        Monday = 1,
        Tuesday = 2,
        Wednesday = 3,
        Thursday = 4,
        Friday = 5,
        Saturday = 6,
        Sunday = 7

    }


}