using System.ComponentModel.DataAnnotations;

namespace ProductService.Models
{
    public class PagedByCompanyUriData
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DisplayLength { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DisplayStart { get; set; }

        [Required]
        [Range(0, 100)]
        public int SortColumn { get; set; }

        [Required]
        public string SortDirection { get; set; }

        public string SearchText { get; set; }
    }
}