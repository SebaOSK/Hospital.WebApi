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
        List<Patient> GetAll();
        List<Patient> GetById(Guid? id);
        bool InsertPatient(Patient newPatient);
        bool UpdatePatient(Guid? id, Patient updatePatient);
        bool Delete(Guid? id);
    }
}
