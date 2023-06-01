using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Hospital.Model;
using System.Linq.Expressions;

namespace Hospital.Repository
{
    public class PatientRepository //: RepositoryCommon.IPatientRepository
    {
        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";
        public List<Patient> GetAll()
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

                    return patientsList;
                }
            }
            catch
            {
                return null;
            }
        }

        public List<Patient> GetById(Guid? id)
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
                catch
                {
                    return null;
                }


            }
        }
        /*public bool InsertPatient();
        public bool UpdatePatient();*/

        public bool DeletePatient(Guid? id)
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

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
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
    }
}


