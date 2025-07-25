namespace AISEA.ApiService.SHARED.Const.Enums
{
    public enum EBookingStatus
    {
        NOT_MATCHED_BOOKING_AVAI = 169,
        PENDING = 1, //VALID TIME TO DO: FUTURE (before 2 days to go), VALID CUR STAT: NONE (Only need valid meeting data like no duplicate, ...)
        CONFIRMED = 2,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 1 day, VALID CUR STAT: PENDING, VALID ACCESS TO THE MEETING
        ADV_CANCELED = 3,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 12hours OR 1 day, VALID CUR STAT: CONFIRMED, VALID ACCESS TO THE MEETING
        /// <summary>
        /// VALID CUR STAT: CONFIRMED || PENDING
        /// case: PENDING then no restriction just make sure current status is actual PENDING then Ban 1
        /// case: CONFIRMED: Before the StartTime of the meeting timeSlot < 1 day (!Not Allowed) => will lead to STUDENT_MISSED mostly then BAN 3
        /// case: CONFIRMED: Before the StartTime of the meeting timeSlot  >= 1 day (OK but ban 2)
        /// if no action occur then the worker will scan then mark the meeting as STUDENT_MISSED
        /// </summary>
        STU_CANCELED = 9,
        COMPLETED = 4,//VALID TIME TO DO: after the StartTime of the meeting TimeSlot (cur not check this for TESTING), VALID CUR STAT: CONFIRMED, VALID CONFIRM CHECK IN CODE, VALID ACCESS TO THE MEETING
        /// <summary>
        /// VALID CUR STAT: CONFIRMED
        /// case: Advisor mark the student missing the meeting VALID TIME TO DO: >= EndTime of the meeting timeSlot (cur not check this for TESTING)
        /// case: Nothing happen when Current Time > EndTime of the meeting timeSlot + 1 day, then the worker service will handle Shift from CONFIRMED  to STUDENT_MISSED statv
        /// VALID ACCESS TO THE MEETING
        /// </summary>
        STUDENT_MISSED = 5,
        ADVISOR_MISSED = 6, //VALID TIME TO DO: After the 15 mins from StartDateTime of the meeting TimeSlot VALID CUR STAT: CONFIRMED, MUST HAVE NOTE, VALID ACCESS TO THE MEETING
        NOT_APPROVED = 7,//VALID TIME TO DO: before the StartTime of the meeting timeSlot 12hours OR 1 day, VALID CUR STAT: PENDING,VALID ACCESS TO THE MEETING
        OVERDUE = 8,// After the EndTime of the meeting timeSlot the worker service will handle shift Stat from PENDING to OVERDUE

        /*

        ///At the same time (Check overload also) A Staff and Student cannot have more than 1 "ACTIVE" Meeting
        ///NON ACTIVE: STU_CANCELED,  NOT_APPROVED, OVERDUE, ADV_CANCELED, NOT_MATCHED_BOOKING_AVAI

        ///  LEAVE NOTE:
        
        ///The advisor can only log leave when there is no meeting in that time slot or if already had then those should be only incase "NON_ACTIVE"

        /// If log leave in truly ACTIVE (not the end of stat flow) meeting then need to take action


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