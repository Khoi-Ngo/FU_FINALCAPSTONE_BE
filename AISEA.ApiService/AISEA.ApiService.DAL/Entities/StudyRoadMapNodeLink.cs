using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AISEA.ApiService.DAL.Entities
{
    public class StudyRoadMapNodeLink
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        public long FromNodeId { get; set; }
        public long ToNodeId { get; set; }

        [ForeignKey(nameof(FromNodeId))]
        public virtual StudyRoadMapNode FromNode { get; set; }

        [ForeignKey(nameof(ToNodeId))]
        public virtual StudyRoadMapNode ToNode { get; set; }
    }
}