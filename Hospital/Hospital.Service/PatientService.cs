using Hospital.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;
using Hospital.Repository;
using Hospital.Common;
using Hospital.RepositoryCommon;

namespace Hospital.Service
{
    public class PatientService : IPatientService
    {
        public PatientService(IPatientRepository patientRepository)
        {
            PatientRepository = patientRepository;
        }

        protected IPatientRepository PatientRepository { get; set; }
        public async Task<PagedList<Patient>> GetAllAsync(Sorting sorting, Filtering filtering, Paging paging)
        {
            //validation for ordering
            string[] orderBy = { "FirstName", "LastName", "DOB", "AppointmentDate", "AppointmentTime", "WardName" };

            if (sorting.OrderBy != null && !orderBy.Contains( sorting.OrderBy))
            {
                throw new Exception();
            }

            PagedList<Patient> result = await PatientRepository.GetAllAsync(sorting, filtering, paging);

            if (result != null)
            {
                return result;
            };
            return null;
        }

        public async Task<List<Patient>> GetByIdAsync(Guid? id)
        {           

            List<Patient> result = await PatientRepository.GetByIdAsync(id);

            if (result != null)
            {
                return result;
            };
            return null;
        }
       
        public async Task<bool> InsertPatientAsync(Patient newPatient)
        {
            

            Guid newGuid = Guid.NewGuid();

            bool isPatient = await PatientRepository.CheckEntryByIdAsync(newGuid);

            if (isPatient)
            {
                return false;
            };

            bool isInserted = await PatientRepository.InsertPatientAsync(newGuid, newPatient);

            if (isInserted)
            { return true; };

            return false;
        }
        public async Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient)
        {
            bool isPatient = await PatientRepository.CheckEntryByIdAsync(id);

            if (isPatient)
            {
                bool isUpdated = await PatientRepository.UpdatePatientAsync(id, updatePatient);
                if (isUpdated)
                { return true; };
            };

            return false;
        }
      
        public async Task<bool> DeletePatientAsync(Guid? id)
        {
            bool isPatient = await PatientRepository.CheckEntryByIdAsync(id);

            if (isPatient)
            {
                bool isDeleted = await PatientRepository.DeletePatientAsync(id);
                if (isDeleted)
                { return true; };
            };
            
            return false;    
        }
    }
}
