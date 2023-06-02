using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;

namespace Hospital.RepositoryCommon
{
    public interface IPatientRepository
    {

        Task<List<Patient>> GetAllAsync();

        List<Patient> GetById(Guid? id);
        Task<bool> InsertPatient(Guid id, Patient newPatient);
        Task<bool> UpdatePatient(Guid? id, Patient updatePatient);
        Task<bool> DeletePatient(Guid? id);
    }
}
