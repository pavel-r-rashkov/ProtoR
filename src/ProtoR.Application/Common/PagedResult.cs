namespace ProtoR.Application.Common
{
    using System.Collections.Generic;

    public class PagedResult<T>
    {
        public PagedResult()
        {
        }

        public PagedResult(int totalCount, IEnumerable<T> items)
        {
            this.TotalCount = totalCount;
            this.Items = items;
        }

        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
