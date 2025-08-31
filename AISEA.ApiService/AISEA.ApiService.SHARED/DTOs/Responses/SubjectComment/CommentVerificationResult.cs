using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AISEA.ApiService.SHARED.DTOs.Responses.SubjectComment
{
    public class CommentVerificationResult
    {
        public bool IsBad { get; set; }
        public string Reason { get; set; }
    }
}