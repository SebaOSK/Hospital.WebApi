using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Hospital.Model;
using System.Linq.Expressions;
using Hospital.RepositoryCommon;

namespace Hospital.Repository
{
    public class PatientRepository : IPatientRepository
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";
        public async Task<List<Patient>> GetAllAsync()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            List<Patient> patientsList = new List<Patient>();

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\"";
                connection.Open();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
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

                return patientsList;
            }
        }

        public List<Patient> GetById(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                connection.Open();
                bool isPatient = CheckEntryById(id);

                if (isPatient)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    NpgsqlDataReader reader = command.ExecuteReader();
                    List<Patient> patientList = new List<Patient>();
                    while (reader.Read())
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
                    return patientList;
                }
                return null;
            }
        }
        public bool InsertPatient(Guid id, Patient newPatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();

                command.Connection = connection;
                command.CommandText = "INSERT INTO \"Hospital\".\"Patient\" VALUES (@Id, @FirstName, @LastName, @DOB, @PhoneNumber, @EmergencyContact)";
                connection.Open();

                bool isPatient = CheckEntryById(newPatient.Id);

                if (isPatient)
                {
                    return false;
                }
                else
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@FirstName", newPatient.FirstName);
                    command.Parameters.AddWithValue("@LastName", newPatient.LastName);
                    command.Parameters.AddWithValue("@DOB", newPatient.DOB);
                    command.Parameters.AddWithValue("@PhoneNumber", newPatient.PhoneNumber);
                    command.Parameters.AddWithValue("@EmergencyContact", newPatient.EmergencyContact);

                    command.ExecuteNonQuery();

                    return true;
                }
            }
        }
        public bool UpdatePatient(Guid? id, Patient updatePatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                bool isPatient = CheckEntryById(id);

                if (isPatient)
                {
                    connection.Open();

                    List<Patient> patientList = new List<Patient>();

                    patientList = GetById(id);


                    StringBuilder updateQuery = new StringBuilder("UPDATE \"Hospital\".\"Patient\" SET ");

                    if (updatePatient.FirstName != null && updatePatient.FirstName != patientList[0].FirstName)
                    {
                        updateQuery.Append("\"FirstName\" = @FirstName ");
                        command.Parameters.AddWithValue("@FirstName", updatePatient.FirstName);
                    };

                    if (updatePatient.LastName != null && updatePatient.LastName != patientList[0].LastName)
                    {
                        updateQuery.Append(", \"LastName\" = @LastName ");
                        command.Parameters.AddWithValue("@LastName", updatePatient.LastName);
                    };

                    if (updatePatient.DOB.HasValue && updatePatient.DOB != patientList[0].DOB)
                    {
                        updateQuery.Append(", \"DOB\" = @DOB ");
                        command.Parameters.AddWithValue("@DOB", updatePatient.DOB);
                    };

                    if (updatePatient.PhoneNumber != null && updatePatient.PhoneNumber != patientList[0].PhoneNumber)
                    {
                        updateQuery.Append(", \"PhoneNumber\" = @PhoneNumber ");
                        command.Parameters.AddWithValue("@PhoneNumber", updatePatient.PhoneNumber);
                    };

                    if (updatePatient.EmergencyContact != null && updatePatient.EmergencyContact != patientList[0].EmergencyContact)
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
                        return true;
                    };

                };
                return false;
            }
        }

        public bool DeletePatient(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                connection.Open();

                bool isPatient = CheckEntryById(id);

                if (isPatient)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;
                    command.CommandText = "DELETE FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    command.ExecuteNonQuery();
                    connection.Close();

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool CheckEntryById(Guid? id)
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
        /*
        private List<Patient> GetPatient(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection();

            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
            command.Parameters.AddWithValue("@Id", id);

            NpgsqlDataReader reader = command.ExecuteReader();
            List<Patient> patientList = new List<Patient>();
            while (reader.Read())
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

            return patientList;

        } */
    }
}


