using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Common;
using Hospital.Model;

namespace Hospital.RepositoryCommon
{
    public interface IPatientRepository
    {

        Task<PagedList<Patient>> GetAllAsync(Sorting sorting, Filtering filtering, Paging paging);

        Task<List<Patient>> GetByIdAsync(Guid? id);
        Task<bool> InsertPatientAsync(Guid id, Patient newPatient);
        Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient);
        Task<bool> DeletePatientAsync(Guid? id);

        Task<bool> CheckEntryByIdAsync(Guid? id);
    }
}
