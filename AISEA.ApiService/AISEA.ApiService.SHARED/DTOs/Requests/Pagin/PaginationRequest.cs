using System.ComponentModel.DataAnnotations;

namespace AISEA.ApiService.SHARED.DTOs.Requests.Pagin;

public class PaginationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 20, ErrorMessage = "PageSize must be between 1 and 20.")]
    public int PageSize { get; set; } = 10;
}