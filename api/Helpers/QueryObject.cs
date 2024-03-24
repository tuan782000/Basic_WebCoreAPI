using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    //Search?key=value - tạo ra object để set key và value
    public class QueryObject
    {
        public string? ProductName { get; set; } = null;
        public string? SortBy {get; set;} = null;
        public bool IsDecsending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}