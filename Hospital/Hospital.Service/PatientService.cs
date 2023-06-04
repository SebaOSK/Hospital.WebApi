using Hospital.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;
using Hospital.Repository;
using Hospital.Common;

namespace Hospital.Service
{
    public class PatientService : IPatientService
    {
        public async Task<PagedList<Patient>> GetAllAsync(Sorting sorting, Filtering filtering, Paging paging)
        {

            PatientRepository patientRepository = new PatientRepository();

            PagedList<Patient> result = await patientRepository.GetAllAsync(sorting, filtering, paging);

            if (result != null)
            {
                return result;
            };
            return null;
        }

        public async Task<List<Patient>> GetByIdAsync(Guid? id)
        {
            PatientRepository patientRepository = new PatientRepository();

            List<Patient> result = await patientRepository.GetByIdAsync(id);

            if (result != null)
            {
                return result;
            };
            return null;
        }
       
        public async Task<bool> InsertPatientAsync(Patient newPatient)
        {

            PatientRepository patientRepository = new PatientRepository();

            Guid newGuid = Guid.NewGuid();

            bool isPatient = await patientRepository.CheckEntryByIdAsync(newGuid);

            if (isPatient)
            {
                return false;
            };

            bool isInserted = await patientRepository.InsertPatientAsync(newGuid, newPatient);

            if (isInserted)
            { return true; };

            return false;
        }
        public async Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient)
        {
            PatientRepository patientRepository = new PatientRepository();
            bool isPatient = await patientRepository.CheckEntryByIdAsync(id);

            if (isPatient)
            {
                bool isUpdated = await patientRepository.UpdatePatientAsync(id, updatePatient);
                if (isUpdated)
                { return true; };
            };

            return false;
        }
      
        public async Task<bool> DeletePatientAsync(Guid? id)
        {
            PatientRepository patientRepository = new PatientRepository();
            bool isPatient = await patientRepository.CheckEntryByIdAsync(id);

            if (isPatient)
            {
                bool isDeleted = await patientRepository.DeletePatientAsync(id);
                if (isDeleted)
                { return true; };
            };
            
            return false;    
        }
    }
}
