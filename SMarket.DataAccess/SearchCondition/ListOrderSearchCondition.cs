namespace SMarket.DataAccess.SearchCondition
{
    public class ListOrderSearchCondition
    {
        public int? PaymentMethodCode { get; set; }
        public int StatusId { get; set; } = 0;
        public int UserId { get; set; } = 0;

        public string Order { get; set; } = "desc";
        public string OrderBy { get; set; } = "CreatedAt";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
