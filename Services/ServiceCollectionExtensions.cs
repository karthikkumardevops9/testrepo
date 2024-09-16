using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MSRecordsEngine.Services.Interface;

namespace MSRecordsEngine.Services
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            //register your service here...
            services.AddTransient<IDataServices, DataServices>();
            services.AddTransient<IReportAAdminService, ReportsAdminService>();
            services.AddTransient<ITrackingServices, TrackingServices>();
            services.AddTransient<IViewService, ViewService>();
            services.AddTransient<IExporterService, ExporterService>();
            services.AddTransient<IBackgroundStatusService, BackgroundStatusService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<ILayoutModelService,LayoutModelService>();
            services.AddTransient<IDatabaseMapService, DatabaseMapService>();
            services.AddTransient<ILayoutDataService, LayoutDataService>();
            services.AddTransient<IDataGridService,  DataGridService>();
            services.AddTransient<IReportsService, ReportsService>();
        }
    }
}
