using AISEA.BgService.Worker.Enums;

namespace AISEA.BgService.Worker.Abstract
{
    public class BaseEntity
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}