using System.ComponentModel.DataAnnotations;
using AISEA.ApiService.SHARED.Const.Enums;

namespace AISEA.ApiService.SHARED.DTOs.Requests.SubjectComment
{
    public class GetSubjectCommentsRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Sort field: Date or LikeCount
        /// </summary>
        public ECommentSortBy SortBy { get; set; } = ECommentSortBy.Date;

        /// <summary>
        /// Sort direction: Asc or Desc
        /// </summary>
        public ESortDirection SortDirection { get; set; } = ESortDirection.Desc;
    }
}
