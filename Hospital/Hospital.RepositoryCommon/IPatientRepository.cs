﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Model;

namespace Hospital.RepositoryCommon
{
    public interface IPatientRepository
    {

        List<Patient> GetAll();

        List<Patient> GetById(Guid? id);
        bool InsertPatient();
        bool UpdatePatient();
        bool DeletePatient();
    }
}
