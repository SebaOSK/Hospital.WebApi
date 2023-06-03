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
using Hospital.Common;
using System.ComponentModel;

namespace Hospital.Repository
{
    public class PatientRepository : IPatientRepository
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";
        public async Task<PagedList<Patient>> GetAllAsync(Paging paging)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            List<Patient> patientsList = new List<Patient>();

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                connection.Open();

                command.CommandText = "SELECT COUNT(\"Id\") FROM \"Hospital\".\"Patient\"";
                object count = await command.ExecuteScalarAsync();
                int entryCount = Convert.ToInt32(count);
                string query = pagingQuery(paging);
                command.CommandText = query;
               
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
                         
                        }) ;
                };
                
                return PagedList<Patient>.ToPagedList(patientsList, paging.pageNumber, paging.pageSize, entryCount);

            }
        }

        public async Task<List<Patient>> GetByIdAsync(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                connection.Open();
                bool isPatient = await CheckEntryByIdAsync(id);

                if (isPatient)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                    command.Parameters.AddWithValue("@Id", id);

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
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
        public async Task<bool> InsertPatientAsync(Guid id, Patient newPatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();

                command.Connection = connection;
                command.CommandText = "INSERT INTO \"Hospital\".\"Patient\" VALUES (@Id, @FirstName, @LastName, @DOB, @PhoneNumber, @EmergencyContact)";
                connection.Open();

                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@FirstName", newPatient.FirstName);
                command.Parameters.AddWithValue("@LastName", newPatient.LastName);
                command.Parameters.AddWithValue("@DOB", newPatient.DOB);
                command.Parameters.AddWithValue("@PhoneNumber", newPatient.PhoneNumber);
                command.Parameters.AddWithValue("@EmergencyContact", newPatient.EmergencyContact);

                await command.ExecuteNonQueryAsync();

                return true;

            }
        }
        public async Task<bool> UpdatePatientAsync(Guid? id, Patient updatePatient)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;


                connection.Open();

                List<Patient> patientList = new List<Patient>();

                patientList = await GetByIdAsync(id);


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

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    return true;
                };

            };
            return false;
        }

        public async Task<bool> DeletePatientAsync(Guid? id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                command.CommandText = "DELETE FROM \"Hospital\".\"Patient\" WHERE \"Id\" = @Id";
                command.Parameters.AddWithValue("@Id", id);

                await command.ExecuteNonQueryAsync();
                connection.Close();

                return true;
            }
        }

        public async Task<bool> CheckEntryByIdAsync(Guid? id)
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

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    return reader.HasRows;

                }
            }
            catch
            {
                return false;
            }

        }

        private string pagingQuery(Paging paging)
        {
            StringBuilder pagingQuery = new StringBuilder("SELECT * FROM \"Hospital\".\"Patient\" ");
           
            if (paging.pageNumber != 1)
            {
                pagingQuery.Append($"OFFSET {(paging.pageNumber - 1 ) * paging.pageSize} "); // sql injection, mislim da ne smije ići sa placeholderima!!
                
            };
            if (paging.pageSize != 3)
            {
                pagingQuery.Append($"FETCH NEXT {paging.pageSize} ROWS ONLY ");
            }
            else
            { pagingQuery.Append("FETCH NEXT 3 ROWS ONLY"); };

            return pagingQuery.ToString();

        }
    }
    
}


