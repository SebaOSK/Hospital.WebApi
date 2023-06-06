using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.WebApi.Models
{
    public class RESTPatient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContact { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }
        public string WardName { get; set; }
        public string ClinicName { get; set; }
    }
}