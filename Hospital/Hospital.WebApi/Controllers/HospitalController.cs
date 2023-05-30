using Hospital.WebApi.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

            return Request.CreateResponse(HttpStatusCode.OK, patientsList);

        }

        // GET: api/Hospital/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Hospital
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Hospital/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Hospital/5
        public void Delete(int id)
        {
        }
    }
}
