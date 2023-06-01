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
    public class PatientService //: IPatientService
    {
        public List<Patient> GetAll()
        {
            PatientRepository patientList = new PatientRepository();

            List<Patient> result = patientList.GetAll();

            if (patientList != null)
            {
                return result;
            };
            return null;
        }

        public List<Patient> GetById(Guid? id)
        {
            PatientRepository patientList = new PatientRepository();

            List<Patient> result = patientList.GetById(id);

            if (patientList != null)
            {
                return result;
            };
            return null;
        }
        /*
        bool InsertPatient(Patient newPatient);
        bool UpdatePatient(Guid? id, Patient updatePatient);
        */
        public bool Delete(Guid? id)
        {
            PatientRepository patientRepository = new PatientRepository();
            bool isDeleted = patientRepository.DeletePatient(id);

            if (isDeleted)
            { return true; };

            return false;
        }
    }
}
