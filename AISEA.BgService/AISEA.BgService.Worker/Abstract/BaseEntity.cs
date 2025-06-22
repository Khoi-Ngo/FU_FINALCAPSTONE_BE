using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.BgService.Worker.Abstract
{
    public class BaseEntity
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? Note { get; set; }
    }
}