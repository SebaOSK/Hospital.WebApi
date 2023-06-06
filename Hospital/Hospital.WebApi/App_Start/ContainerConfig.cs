using Autofac;
using Autofac.Integration.WebApi;
using Hospital.Repository;
using Hospital.RepositoryCommon;
using Hospital.Service;
using Hospital.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;

namespace Hospital.WebApi.App_Start
{
    public class ContainerConfig
    {
        public static void  ConfigureContainer()
        {

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<PatientService>().As<IPatientService>();
            builder.RegisterType<PatientRepository>().As<IPatientRepository>();

            IContainer container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}