using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Common
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> patientsList, int pageNumber, int pageSize, int counter)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int) Math.Ceiling(counter / (double) pageSize);
            TotalCount = counter;
            AddRange(patientsList);
        }

        public static PagedList<T> ToPagedList(List<T> patientsList, int pageNumber, int pageSize, int counter)
        {
            return new PagedList<T>(patientsList, pageNumber, pageSize, counter);
        }
    }
}
