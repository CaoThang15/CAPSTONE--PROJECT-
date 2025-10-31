namespace SMarket.DataAccess.SearchCondition
{
    public class ListOrderSearchCondition
    {
        public int? PaymentMethodCode { get; set; }
        public int StatusId { get; set; } = 0;
        public int Page { get; set; } = 1;
        public string? Keyword { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
