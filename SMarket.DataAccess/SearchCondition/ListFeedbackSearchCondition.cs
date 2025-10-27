namespace SMarket.DataAccess.SearchCondition
{
    public class ListFeedbackSearchCondition
    {
        public int ProductId { get; set; } = 0;
        public int UserId { get; set; } = 0;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
