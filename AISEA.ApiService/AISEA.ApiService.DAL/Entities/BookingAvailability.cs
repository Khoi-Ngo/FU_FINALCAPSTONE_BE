using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("BookingAvailability")]
public class BookingAvailability
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    public DayOfWeek DayInWeek { get; set; }

    public long StaffProfileId { get; set; }

    [ForeignKey("StaffProfileId")]
    [InverseProperty("BookingAvailabilities")]
    public virtual StaffProfile StaffProfile { get; set; } = null!;
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}