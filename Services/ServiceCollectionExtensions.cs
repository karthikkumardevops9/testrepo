using Microsoft.Extensions.DependencyInjection;
using MSRecordsEngine.Services.Interface;

namespace MSRecordsEngine.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            //register your service here...
            services.AddTransient<IDatabaseMap, DatabaseMap>();
            services.AddTransient<IDataServices, DataServices>();
            services.AddTransient<IReportService, ReportsService>();
            services.AddTransient<IExporterService, ExporterService>();
            services.AddTransient<IBackgroundStatusService, BackgroundStatusService>();
        }
    }
}
