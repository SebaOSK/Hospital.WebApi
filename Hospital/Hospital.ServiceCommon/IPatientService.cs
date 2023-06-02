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
        Task<List<Patient>> GetById(Guid? id);
        Task<bool> InsertPatient(Patient newPatient);
        Task<bool> UpdatePatient(Guid? id, Patient updatePatient);
        Task<bool> Delete(Guid? id);
    }
}
