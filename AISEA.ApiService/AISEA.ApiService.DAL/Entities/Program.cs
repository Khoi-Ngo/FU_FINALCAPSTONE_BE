using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;


[Table("Program")]
public partial class Program
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [StringLength(255)]
    public string ProgramName { get; set; } = null;

    [StringLength(50)]
    public string ProgramCode { get; set; } = null;

    public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
}