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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;

namespace Hospital.Repository
{
    public class PatientRepository : IPatientRepository
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";
        public async Task<PagedList<Patient>> GetAllAsync(Sorting sorting, Filtering filtering, Paging paging)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            List<Patient> patientsList = new List<Patient>();

            using (connection)
            {
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;
                connection.Open();

                // a base query, selecting all data from Patient table, and some needed columns from other tables -> Appointment, Ward, Clinic
                StringBuilder baseQuery = new StringBuilder("SELECT \"Patient\".*, \"AppointmentDate\", \"AppointmentTime\", \"WardName\", \"ClinicName\" ");
                baseQuery.Append("FROM \"Hospital\".\"Patient\" ");
                baseQuery.Append("INNER JOIN \"Hospital\".\"Appointment\" ON \"Patient\".\"Id\" = \"Appointment\".\"PatientId\" ");
                baseQuery.Append("INNER JOIN \"Hospital\".\"Ward\" ON \"Appointment\".\"WardId\" = \"Ward\".\"Id\" ");
                baseQuery.Append("INNER JOIN \"Hospital\".\"Clinic\" ON \"Ward\".\"ClinicId\" = \"Clinic\".\"Id\" ");
                baseQuery.Append("WHERE 1 = 1 ");

                //adding filtering options to base query 
                // filter by search query
                if (filtering.SearchQuery != null)
                {
                    baseQuery.Append($"AND (\"FirstName\" ILIKE @search OR \"LastName\" ILIKE @search) ");
                    command.Parameters.AddWithValue("@search", "%" + filtering.SearchQuery + "%");
                };
                //filter by date of birth
                if (filtering.DOB != default)
                {
                        baseQuery.Append("AND \"DOB\"  = @dob ");
                        command.Parameters.AddWithValue("@dob", filtering.DOB);                                        
                };
                // filter by from - to date of birth
                if (filtering.FromDate != default && filtering.ToDate != default)
                {
                        baseQuery.Append("AND \"DOB\" BETWEEN @fromDate AND @toDate ");
                        command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        command.Parameters.AddWithValue("@toDate", filtering.ToDate);                    
                }
                else
                {
                    if (filtering.FromDate != default)
                    {                   
                            baseQuery.Append("AND \"DOB\" > @fromDate ");
                            command.Parameters.AddWithValue("@fromDate", filtering.FromDate);                   
                    };
                    if (filtering.ToDate != default)
                    {                    
                            baseQuery.Append("AND \"DOB\" < @toDate ");
                            command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                    };
                };
              
                // getting the number of entries for submitting via PagedList
                string countQuery = baseQuery.ToString();
                string queryRemove = countQuery.Remove(7, 76);
                string count = queryRemove.Insert(7, "COUNT(\"Patient\".\"Id\") ");
                command.CommandText = count;
                int entryCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                //adding sorting options to base query 
                if (sorting.OrderBy != null)
                {
                    baseQuery.Append($"ORDER BY \"{sorting.OrderBy}\" ");
                    if (sorting.SortOrder != null)
                    {
                        baseQuery.Append("DESC ");
                    };
                };

                // adding paging options base query
                if (paging.PageNumber != 1)
                {
                    baseQuery.Append($"OFFSET @offset ");
                    command.Parameters.AddWithValue("@offset", (paging.PageNumber - 1) * paging.PageSize);
                };
                if (paging.PageSize != 3)
                {
                    baseQuery.Append($"FETCH NEXT @next ROWS ONLY;");
                    command.Parameters.AddWithValue("@next", paging.PageSize);
                }
                else
                { baseQuery.Append("FETCH NEXT @default ROWS ONLY;");
                    command.Parameters.AddWithValue("@default", paging.PageSize);
                };

                // converting baseQuery to string and executing it                                
                string query = baseQuery.ToString();
                command.CommandText = query;
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                //adding result of query to patientsList
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
                            AppointmentDate = (DateTime)reader["AppointmentDate"],
                            AppointmentTime = (TimeSpan)reader["AppointmentTime"],
                            WardName = (string)reader["WardName"],
                            ClinicName = (string)reader["ClinicName"]

                        });
                };

                return PagedList<Patient>.ToPagedList(patientsList, paging.PageNumber, paging.PageSize, entryCount);

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
    }
}


