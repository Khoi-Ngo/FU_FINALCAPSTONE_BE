namespace AISEA.ApiService.SHARED.Const.Enums
{
    public enum EBookingStatus
    {
        PENDING = 1,
        CONFIRMED = 2,
        ADV_CANCELED = 3,
        STU_CANCELED = 9,
        COMPLETED = 4,
        STUDENT_MISSED = 5,
        ADVISOR_MISSED = 6,
        NOT_APPROVED = 7,
        OVERDUE = 8,

        //?The student can only book when there is no existed meeting in that time slot or STU_CANCELED only (Handle By Trigger Database)

        //?The advisor can only log leave when there is no meeting in that time slot or if already had then those should be only incase (NOT_APPROVE, ADV_CANCELED, STU_CANCEL)

        //? Other case need Advisor take action (Cancel them or do something similar like that ...) to be able to complete the logging leave (PENDING, CONFIRMED)

        //? Other case will never happen if handle right(because only allow the advisor logging leave for the Time in the future): Create Leave when there is meeting (OVERDUE, ADVISOR_MISSED, STUDENT_MISSED, COMPLETED, )
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