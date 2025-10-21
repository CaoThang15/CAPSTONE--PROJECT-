namespace SMarket.DataAccess.SearchCondition
{
    public class ListProductSearchCondition
    {
        public int CategoryId { get; set; } = 0;
        public string? KeyWord { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsHide { get; set; }
        public bool? IsAdminHide { get; set; }
        public int SellerId { get; set; } = 0;

        public string Order { get; set; } = "desc";
        public string OrderBy { get; set; } = "CreatedAt";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
