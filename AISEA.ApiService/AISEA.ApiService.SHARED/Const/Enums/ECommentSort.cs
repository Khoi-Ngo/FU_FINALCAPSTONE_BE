using System.ComponentModel;

namespace AISEA.ApiService.SHARED.Const.Enums
{
    /// <summary>
    /// Sort fields for comments
    /// </summary>
    public enum ECommentSortBy
    {
        /// <summary>
        /// Sort by creation date
        /// </summary>
        [Description("Sort by creation date")]
        Date = 1,

        /// <summary>
        /// Sort by like count (popularity)
        /// </summary>
        [Description("Sort by like count (popularity)")]
        LikeCount = 2
    }

    /// <summary>
    /// Sort direction
    /// </summary>
    public enum ESortDirection
    {
        /// <summary>
        /// Ascending order (oldest/lowest first)
        /// </summary>
        [Description("Ascending order (oldest/lowest first)")]
        Asc = 1,

        /// <summary>
        /// Descending order (newest/highest first)
        /// </summary>
        [Description("Descending order (newest/highest first)")]
        Desc = 2
    }
}
