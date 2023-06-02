using Hospital.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;
using Hospital.Repository;

namespace Hospital.Service
{
    public class PatientService : IPatientService
    {
        public async Task<List<Patient>> GetAllAsync()
        {
            PatientRepository patientList = new PatientRepository();

            List<Patient> result = await patientList.GetAllAsync();

            if (patientList != null)
            {
                return result;
            };
            return null;
        }

        public async Task<List<Patient>> GetByIdAsync(Guid? id)
        {
            PatientRepository patientList = new PatientRepository();

            List<Patient> result = await patientList.GetByIdAsync(id);

            if (patientList != null)
            {
                return result;
            };
            return null;
        }
       
        public async Task<bool> InsertPatientAsync(Patient newPatient)
        {
            PatientRepository patientList = new PatientRepository();

            Guid newGuid = Guid.NewGuid();

            bool result = await patientList.InsertPatientAsync(newGuid, newPatient);



            if (result)
            {
                return true;
            };

            return false;
        }
        public async Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient)
        {
            PatientRepository patientRepository = new PatientRepository();
            bool isUpdated = await patientRepository.UpdatePatientAsync(id, updatePatient);

            if (isUpdated)
            { return true; };

            return false;
        }
      
        public async Task<bool> DeletePatientAsync(Guid? id)
        {
            PatientRepository patientRepository = new PatientRepository();
            bool isDeleted = await patientRepository.DeletePatientAsync(id);

            if (isDeleted)
            { return true; };

            return false;
        }
    }
}
