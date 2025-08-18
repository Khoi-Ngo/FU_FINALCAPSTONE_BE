using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities;

[Table("OptionalSubjectCheckPoint")]
public class OptionalSubjectCheckPoint
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    public string Title { get; set; }
    public string? Content { get; set; }
    public string? Note { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string? Link1 { get; set; }
    public string? Link2 { get; set; }
    public string? Link3 { get; set; }
    public string? Link4 { get; set; }
    public string? Link5 { get; set; }
    public DateTime Deadline { get; set; }


    // Foreign key to OptionalPersonalSubject
    [ForeignKey("OptionalPersonalSubject")]
    public long OptionalPersonalSubjectId { get; set; }

    // Navigation property
    [InverseProperty("Checkpoints")]
    public virtual OptionalPersonalSubject OptionalPersonalSubject { get; set; } = null!;
}
