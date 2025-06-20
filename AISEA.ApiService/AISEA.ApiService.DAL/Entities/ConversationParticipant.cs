using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AISEA.ApiService.DAL.Abstract;

namespace AISEA.ApiService.DAL.Entities;

[Table("ConversationParticipant")]
public partial class ConversationParticipant : BaseEntity
{
    [Key]
    [Column("user_id")]
    public long UserId { get; set; }

    [Key]
    [Column("conversation_id")]
    public long ConversationId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ConversationParticipants")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("ConversationId")]
    [InverseProperty("ConversationParticipants")]
    public virtual Conversation Conversation { get; set; } = null!;
}