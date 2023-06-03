using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.Common
{
    public class PagedList<Patient> : List<Patient>
    {
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }

        public int TotalCount { get; private set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<Patient> patientsList, int pageNumber, int pageSize, int count)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
            AddRange(patientsList);
        }

        public static PagedList<Patient> ToPagedList(List<Patient> patientsList, int pageNumber, int pageSize, int count)
        {
            return new PagedList<Patient>(patientsList, pageNumber, pageSize, count);
        }
    }
}
