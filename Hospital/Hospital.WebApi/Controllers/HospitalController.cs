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
            PatientService patientsService = new PatientService();

            List<Patient> result = patientsService.GetAll();

            if (result != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, result);
            };

            return Request.CreateResponse(HttpStatusCode.NotFound, "No entries in database!");
        }

        // implement get methods so that they send evrything except Id(RESTPatient)

        // GET: api/Hospital/5
        public HttpResponseMessage Get(Guid? id)
        {
            PatientService patientService = new PatientService();
            List<Patient> result = patientService.GetById(id);

            if (result != null)
            { return Request.CreateResponse(HttpStatusCode.OK, result);
            };

            return Request.CreateResponse(HttpStatusCode.NotFound);

        }
    

        /*/ POST: api/Hospital
        public HttpResponseMessage Post([FromBody]RESTPatient newPatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand();

                    command.Connection = connection;
                    command.CommandText = "INSERT INTO \"Hospital\".\"Patient\" VALUES (@Id, @FirstName, @LastName, @DOB, @PhoneNumber, @EmergencyContact)";
                    connection.Open();

                    bool patient = CheckEntryById(newPatient.Id);
                    Guid newId = Guid.NewGuid();

                    if (patient)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, $"Patient {newPatient.FirstName} {newPatient.LastName} already in database");
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@Id", newId);
                        command.Parameters.AddWithValue("@FirstName", newPatient.FirstName);
                        command.Parameters.AddWithValue("@LastName", newPatient.LastName);
                        command.Parameters.AddWithValue("@DOB", newPatient.DOB);
                        command.Parameters.AddWithValue("@PhoneNumber", newPatient.PhoneNumber);
                        command.Parameters.AddWithValue("@EmergencyContact", newPatient.EmergencyContact);

                        command.ExecuteNonQuery();

                        return Request.CreateResponse(HttpStatusCode.OK, $"Patient {newPatient.FirstName} {newPatient.LastName} added!!");
                    }
                }

                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooop, something went wrong");
                }
            }
                
    
          

        }

        // PUT: api/Hospital/5
        //create a list to check if new data is same as old, otherwise Response HttpStatusCode.NotFound not useless
        public HttpResponseMessage Put(Guid? id, [FromBody]RESTPatient updatePatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using(connection)
            {
                try
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    bool patient = CheckEntryById(id);

                    if (patient)
                    {
                        connection.Open();

                        StringBuilder updateQuery = new StringBuilder("UPDATE \"Hospital\".\"Patient\" SET ");

                        if (updatePatient.FirstName != null)
                        {
                            updateQuery.Append("\"FirstName\" = @FirstName ");
                            command.Parameters.AddWithValue("@FirstName", updatePatient.FirstName);
                        };

                        if (updatePatient.LastName != null)
                        {
                            updateQuery.Append(", \"LastName\" = @LastName ");
                            command.Parameters.AddWithValue("@LastName", updatePatient.LastName);
                        };

                        if (updatePatient.DOB.HasValue)
                        {
                            updateQuery.Append(", \"DOB\" = @DOB ");
                            command.Parameters.AddWithValue("@DOB", updatePatient.DOB);
                        };

                        if (updatePatient.PhoneNumber != null)
                        {
                            updateQuery.Append(", \"PhoneNumber\" = @PhoneNumber ");
                            command.Parameters.AddWithValue("@PhoneNumber", updatePatient.PhoneNumber);
                        };

                        if (updatePatient.EmergencyContact != null)
                        {
                            updateQuery.Append(", \"EmergencyContact\" = @EmergencyContact ");
                            command.Parameters.AddWithValue("@EmergencyContact", updatePatient.EmergencyContact);
                        }

                        updateQuery.Append("WHERE \"Id\" = @id");
                        command.Parameters.AddWithValue("@id", id);

                        string query = updateQuery.ToString();

                        query = query.Replace("SET ,", "SET ");
                        command.CommandText = query;
                       
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Entry updated!!"); //Get(id)
                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.NotModified);
                        } 
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Patient not found!");
                    }
                 
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooop, something went wrong");
                }
            }
        }*/

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
        
            

        

        
     