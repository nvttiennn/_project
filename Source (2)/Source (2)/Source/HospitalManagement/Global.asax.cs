using BELibrary.DbContext;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace HospitalManagement
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Database.SetInitializer<HospitalManagementDbContext>(new DropCreateDatabaseIfModelChanges<HospitalManagementDbContext>());
            Database.SetInitializer<HospitalManagementDbContext>(null);
        }
    }
}