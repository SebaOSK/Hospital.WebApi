using Hospital.Model;
using Hospital.Service;
using Hospital.WebApi.Models;
using Microsoft.Ajax.Utilities;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;

namespace Hospital.WebApi.Controllers
{
    public class HospitalController : ApiController
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";

        // GET: api/Hospital
        public HttpResponseMessage Get()
        {
            try
            {
                PatientService patientsService = new PatientService();

                List<Patient> result = patientsService.GetAll();

                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                };

                return Request.CreateResponse(HttpStatusCode.NotFound, "No entries in database!");

            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong");
            }   
        }

        // implement get methods so that they send evrything except Id(RESTPatient)

        // GET: api/Hospital/5
        public HttpResponseMessage Get(Guid? id)
        {
            try
            {
                PatientService patientService = new PatientService();
                List<Patient> result = patientService.GetById(id);

                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                };

                return Request.CreateResponse(HttpStatusCode.NotFound, "Entry not found!!");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }                        
        }


        // POST: api/Hospital
        public HttpResponseMessage Post([FromBody] Patient newPatient)
        {
            PatientService patientService = new PatientService();
            bool isAdded = patientService.InsertPatient(newPatient);

            if (isAdded)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Entry added!!");
            };

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
        }   

        // PUT: api/Hospital/5
        //create a list to check if new data is same as old, otherwise Response HttpStatusCode.NotFound not useless
        public HttpResponseMessage Put(Guid? id, [FromBody] Patient updatePatient)
        {
            PatientService patientService = new PatientService();
            bool isDeleted = patientService.UpdatePatient(id, updatePatient);

            if (isDeleted)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Entry updated");
            };

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
        }

        // DELETE: api/Hospital/5
        public HttpResponseMessage Delete(Guid? id)
        {
            PatientService patientService = new PatientService();
            bool isDeleted = patientService.Delete(id);

            if (isDeleted)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Entry deleted");
            };

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!"); 
        }

     
  }
}
        
            

        

        
     