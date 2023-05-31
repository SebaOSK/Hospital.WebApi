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
                    connection.Close();
                }
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooop, something went wrong");
            }
            

            return Request.CreateResponse(HttpStatusCode.OK, patientsList);

        }

        // GET: api/Hospital/5
        public HttpResponseMessage Get(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                try
                {
                    connection.Open();
                    bool patient = CheckEntryById(id);

                    if (patient)
                    {
                        List<Patient> patientList = new List<Patient>();

                        NpgsqlCommand command = new NpgsqlCommand();
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);

                        NpgsqlDataReader reader = command.ExecuteReader();

                        while(reader.Read())
                        {
                            patientList.Add(
                            new Patient()
                            {

                                Id = (Guid)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                DOB = (DateTime)reader["DOB"],
                                PhoneNumber = (string)reader["PhoneNumber"],
                                EmergencyContact = (string)reader["EmergencyContact"],

                            });
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, patientList[0]);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Patient entry not found");
                    }
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooop, something went wrong");
                }
            }

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
        public HttpResponseMessage Put(Guid? id, [FromBody]Patient updatePatient)
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

                        command.CommandText = updateQuery.ToString();
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Get(id); //Request.CreateResponse(HttpStatusCode.OK, "Entry updated!!");
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
        }

        // DELETE: api/Hospital/5
        public HttpResponseMessage Delete(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                try 
                {
                    List<Patient> deletedPatientList = new List<Patient>();
                    
                    connection.Open();

                    bool patient = CheckEntryById(id);

                    if (patient)
                    {
                        NpgsqlCommand command = new NpgsqlCommand();
                        command.Connection = connection;
                        command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);
                        NpgsqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            deletedPatientList.Add(
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
                        reader.Close();

                        command.CommandText = "DELETE FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                        command.Parameters.AddWithValue("@Id", id);

                        command.ExecuteNonQuery();
                        connection.Close();

                        return Request.CreateResponse(HttpStatusCode.OK, $"DELETED: {deletedPatientList[0].FirstName}, {deletedPatientList[0].LastName}, {deletedPatientList[0].DOB}");
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Patient entry not found");
                    }
                }
                catch
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooop, something went wrong");
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

                    return reader.HasRows;

                }
            }
            catch
            {
                return false;
            }
               
        }

    }
}
