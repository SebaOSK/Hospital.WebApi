using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.WebApi.Models
{
    public class RESTPagedList
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public List<RESTPatient> Data { get; set; }
    }
}