namespace SMarket.Business.DTOs.Common
{
    public class PaginationMetadata
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int Page { set; get; }

        /// <summary>
        /// Tổng số trang hiển thị
        /// </summary>
        public int TotalPages { set; get; }

        /// <summary>
        /// Số dòng trên 1 trang
        /// </summary>
        public int PageSize { set; get; }

        /// <summary>
        /// Tổng số các dòng
        /// </summary>
        public int TotalItems { set; get; }

        /// <summary>
        /// Còn dữ liệu để xem tiếp không
        /// </summary>
        public bool HasMoreRecords { get; set; }
    }

    public class PaginationResult<TEntity> where TEntity : class
    {
        public PaginationMetadata Metadata { get; set; }
        public IEnumerable<TEntity> Items { get; set; }

        public PaginationResult()
        {
            Metadata = new PaginationMetadata();
            Items = Enumerable.Empty<TEntity>();
        }

        public PaginationResult(
                  int currentPage,
                  int pageSize,
                  int totalItems,
                  IEnumerable<TEntity> items)
        {
            this.Items = items ?? Enumerable.Empty<TEntity>();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            Metadata = new PaginationMetadata
            {
                Page = currentPage,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                HasMoreRecords = currentPage < totalPages
            };
        }

        public static PaginationResult<TEntity> Empty => new PaginationResult<TEntity>();

    }
}