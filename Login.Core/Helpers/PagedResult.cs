using System.Collections.Generic;

namespace Login.Core.Helpers
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Result { get; set; }
        public int TotalPages { get; set; }
    }
}