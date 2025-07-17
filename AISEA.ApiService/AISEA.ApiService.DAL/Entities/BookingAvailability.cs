using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.DAL.Entities;

[Table("BookingAvailability")]
public class BookingAvailability
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public DayOfWeekAISEA DayInWeek { get; set; }

    public long StaffProfileId { get; set; }

    [ForeignKey("StaffProfileId")]
    [InverseProperty("BookingAvailabilities")]
    public virtual StaffProfile StaffProfile { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}