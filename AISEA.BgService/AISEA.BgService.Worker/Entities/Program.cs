using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.BgService.Worker.Abstract;

namespace AISEA.BgService.Worker.Entities;


[Table("Program")]
public partial  class Program : BaseEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    [StringLength(255)]
    public string ProgramName { get; set; } = null;
    
    [StringLength(50)]
    public string ProgramCode { get; set; } = null;
    
    public virtual ICollection<Curriculum> Curricula { get; set; } = new List<Curriculum>();
}