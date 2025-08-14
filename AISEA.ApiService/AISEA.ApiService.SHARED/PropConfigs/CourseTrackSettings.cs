namespace AISEA.ApiService.SHARED.PropConfigs;

public class CourseTrackSettings
{
    public const string Section = "CourseTrackSettings";

    #region Semester
    //JUST Get Month and Day the Year must be checked by Current DateTime
    public DateOnly SpringSemesterStartDate { get; set; }
    public DateOnly SpringSemesterEndDate { get; set; }
    public DateOnly SummerSemesterStartDate { get; set; }
    public DateOnly SummerSemesterEndDate { get; set; }
    public DateOnly FallSemesterStartDate { get; set; }
    public DateOnly FallSemesterEndDate { get; set; }
    public int AddSemesterNameIntervalDays { get; set; }

    #endregion
    public int MaxDuplicateSubjectCodePerStuSem { get; set; }
    public int RemoveNonUseJoinedSubjectIntervalDays { get; set; }


}