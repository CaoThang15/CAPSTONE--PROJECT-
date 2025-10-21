namespace SMarket.Business.DTOs.Common
{
    public class Paging
    {
        /// <summary>
        /// Trang hiện tại
        /// </summary>
        public int CurrentPage { set; get; }

        /// <summary>
        /// Tổng số trang hiển thị
        /// </summary>
        public int TotalPages { set; get; }

        /// <summary>
        /// Số dòng tối đa trên 1 trang
        /// </summary>
        public int NumberOfRecord { set; get; }

        /// <summary>
        /// Tổng số các dòng
        /// </summary>
        public int TotalRecord { set; get; }

        public Paging()
        {
            NumberOfRecord = 10;
            TotalPages = 1;
            CurrentPage = 1;
            TotalRecord = 0;
        }

        public Paging(int TotalRecord, int CurrentPage, int NumberOfRecord = 30)
        {
            this.TotalRecord = TotalRecord;
            this.NumberOfRecord = NumberOfRecord;
            if (this.NumberOfRecord == 0)
            {
                this.NumberOfRecord = TotalRecord;
            }
            this.TotalPages = TotalRecord / this.NumberOfRecord + (TotalRecord % this.NumberOfRecord > 0 ? 1 : 0);
            if (CurrentPage > this.TotalPages)
            {
                CurrentPage = this.TotalPages == 0 ? 1 : this.TotalPages;
            }
            if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            this.CurrentPage = CurrentPage;
        }
    }

    public class PagingResult<TEntity> where TEntity : class
    {
        public TEntity[] Items { get; set; }

        public bool HasMoreRecords { get; set; }

        public int Total { get; set; }

        public static PagingResult<TEntity> Empty
        {
            get
            {
                return new PagingResult<TEntity>
                {
                    Items = EmptyArray<TEntity>.Instance,
                };
            }
        }
    }

    public static class EmptyArray<T>
    {
        private static readonly T[] InstanceInternal = new T[0];

        public static T[] Instance
        {
            get { return InstanceInternal; }
        }
    }
}