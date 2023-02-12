using AdvanceAPI.API.Models;

namespace AdvanceAPI.Models
{
    public class ProductQueryParameters : QueryParameters
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
