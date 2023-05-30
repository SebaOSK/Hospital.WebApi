using Hospital.WebApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Hospital.WebApi.Controllers
{
    public class HospitalController : ApiController
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";

        // GET: api/Hospital
        public HttpResponseMessage Get()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            List<Patient> patientsList = new List<Patient>();
            try
                {
                using (connection)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\"";
                    connection.Open();

                    NpgsqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        patientsList.Add(
                            new Patient()
                            {

                                Id = (Guid)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                DOB = (DateTime)reader["DOB"],
                                PhoneNumber = (string)reader["PhoneNumber"],
                                EmergencyContact = (string)reader["EmergencyContact"],

                            });

                    };
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            

            return Request.CreateResponse(HttpStatusCode.OK, patientsList);

        }

        // GET: api/Hospital/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Hospital
        public HttpResponseMessage Post([FromBody]Patient newPatient)
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

                        return Request.CreateResponse(HttpStatusCode.OK, "Patient added!!");
                    }
                }

                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }
                
    
          

        }

        // PUT: api/Hospital/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Hospital/5
        public HttpResponseMessage Delete(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                try 
                {
                    connection.Open();
                    bool result = CheckEntryById(id);

                    if (result)
                    {
                        NpgsqlCommand command = new NpgsqlCommand();
                        command.Connection = connection;
                        command.CommandText = "DELETE FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);

                        command.ExecuteNonQuery();
                        connection.Close();

                        return Request.CreateResponse(HttpStatusCode.OK, "Entry deleted!");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Patient entry not found");
                    }
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
            }

        }

        public bool CheckEntryById(Guid? id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);

                using (connection)
                {
                    connection.Open();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    string query = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @id;";
                    command.Parameters.AddWithValue("@id", id);
                    command.CommandText = query;
                     
                    NpgsqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    { return true; }
                    else
                    { return false; }

                }
            }
            catch
            {
                return false;
            }
               
        }

    }
}
