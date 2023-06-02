using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;

namespace Hospital.ServiceCommon
{
    public interface IPatientService
    {
        Task<List<Patient>> GetAllAsync();
        Task<List<Patient>> GetByIdAsync(Guid? id);
        Task<bool> InsertPatientAsync(Patient newPatient);
        Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient);
        Task<bool> DeletePatientAsync(Guid? id);
    }
}
