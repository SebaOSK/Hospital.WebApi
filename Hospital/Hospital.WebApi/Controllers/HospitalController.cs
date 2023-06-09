﻿using Hospital.Common;
using Hospital.Model;
using Hospital.Service;
using Hospital.ServiceCommon;
using Hospital.WebApi.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
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
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls.WebParts;

namespace Hospital.WebApi.Controllers
{
    public class HospitalController : ApiController
    {
        public HospitalController(IPatientService patientService) 
        {
            PatientService = patientService;
        }

        protected IPatientService PatientService { get; set; }

        static string connectionString = "Server=localhost;Port=5432;User Id=postgres;Password=jebeniPOSTgreSQL;Database=postgres";

        // GET: api/Hospital
        public async Task<HttpResponseMessage> GetAsync(int pageNumber = 1, int pageSize = 3,
                                                        string orderBy = null, string sortOrder = null,
                                                        string searchQuery = null, DateTime? dob = null,
                                                        DateTime? fromDate = null, DateTime? toDate = null,
                                                        TimeSpan? fromTime = default, TimeSpan? toTime = default)
        {
            try
            {
                Filtering filtering = new Filtering()
                {
                    SearchQuery = searchQuery,
                    DOB = dob.HasValue ? (DateTime)dob : default,
                    FromDate = fromDate.HasValue ? (DateTime)fromDate : default,
                    ToDate = toDate.HasValue ? (DateTime)toDate : default,
                    FromTime = fromTime.HasValue ? (TimeSpan)fromTime : default,
                    ToTime = toTime.HasValue ? (TimeSpan)toTime : default,
                };
                
                Sorting sorting = new Sorting() { OrderBy = orderBy, SortOrder = sortOrder };

                Paging paging = new Paging() { PageNumber = pageNumber, PageSize = pageSize };

               

                PagedList<Patient> result = await PatientService.GetAllAsync(sorting, filtering, paging);

                if (result != null)
                {

                    List<RESTPatient> restPatient = new List<RESTPatient>();

                    for (int counter = 0; counter < result.Count(); counter++)
                    {
                        restPatient.Add(new RESTPatient()
                        {
                            FirstName = result[counter].FirstName,
                            LastName = result[counter].LastName,
                            DOB = result[counter].DOB,
                            PhoneNumber = result[counter].PhoneNumber,                            
                            EmergencyContact = result[counter].EmergencyContact,
                            AppointmentDate = result[counter].AppointmentDate,
                            AppointmentTime = result[counter].AppointmentTime,
                            WardName = result[counter].WardName,
                            ClinicName = result[counter].ClinicName
                        });
                    };

                    List<RESTPagedList> data = new List<RESTPagedList>
                    {
                        new RESTPagedList()
                        {
                            CurrentPage = result.CurrentPage,
                            PageSize = result.PageSize,
                            TotalPages = result.TotalPages,
                            TotalCount = result.TotalCount,
                            HasPrevious = result.HasPrevious,
                            HasNext = result.HasNext,
                            Data = restPatient
                        }
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, data);

                }
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No entries in database!");
                }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong");
            }
        }

        // implement get methods so that they send everything except Id(RESTPatient)

        // GET: api/Hospital/5
        public async Task<HttpResponseMessage> GetAsync(Guid? id)
        {
            try
            {
                List<Patient> result = await PatientService.GetByIdAsync(id);

                List<RESTPatient> restPatient = new List<RESTPatient>();

                for (int counter = 0; counter < result.Count(); counter++)
                {
                    restPatient.Add(new RESTPatient()
                    {
                        FirstName = result[counter].FirstName,
                        LastName = result[counter].LastName,
                        DOB = result[counter].DOB,
                        PhoneNumber = result[counter].PhoneNumber,
                        EmergencyContact = result[counter].EmergencyContact
                    });
                };

                if (result != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, restPatient);
                };

                return Request.CreateResponse(HttpStatusCode.NotFound, "Entry not found!!");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }
        }


        // POST: api/Hospital
        public async Task<HttpResponseMessage> PostAsync([FromBody] Patient newPatient)
        {
            try
            {
                bool isInserted = await PatientService.InsertPatientAsync(newPatient);

                if (isInserted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Entry added!!");
                };
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Patient already in database");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }
        }

        // PUT: api/Hospital/5
        //create a list to check if new data is same as old, otherwise Response HttpStatusCode.NotFound not useless
        public async Task<HttpResponseMessage> PutAsync(Guid? id, [FromBody] Patient updatePatient)
        {
            try
            {
                
                bool isUpdated = await PatientService.UpdatePatientAsync(id, updatePatient);

                if (isUpdated)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Entry updated");
                };
                return Request.CreateResponse(HttpStatusCode.NotFound, "Patient not found!!");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }
        }

        // DELETE: api/Hospital/5
        public async Task<HttpResponseMessage> DeleteAsync(Guid? id)
        {
            try
            {
                bool isDeleted = await PatientService.DeletePatientAsync(id);

                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Entry deleted");
                };

                return Request.CreateResponse(HttpStatusCode.NotFound, "Patient not found!!");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Ooops, something went wrong!!");
            }
        }
    }
}






