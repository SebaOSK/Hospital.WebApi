using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.ModelCommon
{
    public interface IPatient
    {
        Guid Id { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        DateTime? DOB { get; set; }
        string PhoneNumber { get; set; }
        string EmergencyContact { get; set; }

        DateTime? AppointmentDate { get; set; }
        TimeSpan? AppointmentTime { get; set; }
        string WardName { get; set; }
        string ClinicName { get; set; }
    }
}
