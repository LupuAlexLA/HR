using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.Antiforgery;
using Abp.AspNetCore.SignalR.Hubs;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Castle.Facilities.Logging;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DocumentOperationServiceSample.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Niva.Erp.Configuration;
using Niva.Erp.Conta.Reports;
using Niva.Erp.ExternalApi;
using Niva.Erp.Identity;
using Niva.Erp.Web.Host.Reports.Common;
using Niva.Erp.Web.Host.Reports.Conta;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Niva.Erp.Web.Host.Startup
{
    public class Startup
    {
        private const string _defaultCorsPolicyName = "localhost";

        private const string _apiVersion = "v1";

        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Register DevExpressControls
            services.AddDevExpressControls();

            services.AddSingleton<IScopedTestReportDataSourceProvider<ReportsAppService>, ScopedTestReportDataSourceProvider<ReportsAppService>>();

            services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
            services.AddTransient<TestReportDataSource>();
            services.AddScoped<IObjectDataSourceInjector, ObjectDataSourceInjector>();
            services.AddScoped<IWebDocumentViewerReportResolver, WebDocumentViewerReportResolver>();
            services.AddScoped<PreviewReportCustomizationService, CustomPreviewReportCustomizationService>();
            services.AddScoped<DocumentOperationService, CustomDocumentOperationService>();
            services.AddScoped<IAngajatiExternManager, AngajatiExternManager>();
            services.AddScoped<IEmitentiExternManager, EmitentiExternManager>();
            services.AddScoped<IActiveBugetBVCManager, ActiveBugetBVCManager>();
            services.AddScoped<IPlasamentBNRManager, PlasamentBNRManager> ();
            services.AddScoped<IPlasamentLichiditateManager, PlasamentLichiditateManager>();
            //MVC
            services
            .AddControllersWithViews(
                options =>
                {
                    options.Filters.Add(new AbpAutoValidateAntiforgeryTokenAttribute());
                }
            )
            .AddDefaultReportingControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new AbpMvcContractResolver(IocManager.Instance)
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                };
            });
            services.ConfigureReportingServices(configurator =>
            {
                configurator.ConfigureReportDesigner(designerConfigurator =>
                {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
                    designerConfigurator.RegisterObjectDataSourceWizardTypeProvider<ObjectDataSourceWizardCustomTypeProvider>();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator =>
                {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });

            IdentityRegistrar.Register(services);
            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddSignalR();

            // Configure CORS for angular2 UI
            services.AddCors(
                options => options.AddPolicy(
                    _defaultCorsPolicyName,
                    builder => builder
                        .WithOrigins(
                            // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                            _appConfiguration["App:CorsOrigins"]
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                )
            );

            // Swagger - Enable this line and the related lines in Configure method to enable swagger UI
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(_apiVersion, new OpenApiInfo
                {
                    Version = _apiVersion,
                    Title = "Erp API",
                    Description = "Erp",
                    // uncomment if needed TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Erp",
                        Email = string.Empty,
                        Url = new Uri("https://twitter.com/aspboilerplate"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://github.com/aspnetboilerplate/aspnetboilerplate/blob/dev/LICENSE"),
                    }
                });
                options.DocInclusionPredicate((docName, description) =>
                {
                    if (description.ActionDescriptor.DisplayName == "Niva.Erp.Web.Host.Controllers.ReportDesignerController.BalanceReport (Niva.Erp.Web.Host)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.WebDocumentViewer.WebDocumentViewerController.Invoke (DevExpress.AspNetCore.Reporting.v20.2)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.WebDocumentViewer.WebDocumentViewerController.GetLocalization (DevExpress.AspNetCore.Reporting.v20.2)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.ReportDesigner.ReportDesignerController.Invoke (DevExpress.AspNetCore.Reporting.v20.2)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.ReportDesigner.ReportDesignerController.GetLocalization (DevExpress.AspNetCore.Reporting.v20.2)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.QueryBuilder.QueryBuilderController.Invoke (DevExpress.AspNetCore.Reporting.v20.2)"
                        || description.ActionDescriptor.DisplayName == "DevExpress.AspNetCore.Reporting.QueryBuilder.QueryBuilderController.GetLocalization (DevExpress.AspNetCore.Reporting.v20.2)")
                    {
                        return false;
                    }
                    return true;
                });

                // Define the BearerAuth scheme that's in use
                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
            });

            // Configure Abp and Dependency Injection
            return services.AddAbp<ErpWebHostModule>(
                // Configure Log4Net logging
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                )
            );


        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IWebHostEnvironment env)
        {
            app.UseAbp(options => { options.UseAbpRequestLocalization = false; }); // Initializes ABP framework.

            app.UseCors(_defaultCorsPolicyName); // Enable CORS!

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) && !context.Request.Path.Value.StartsWith("/api/services", StringComparison.InvariantCultureIgnoreCase))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });



            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAbpRequestLocalization();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "dist"))
            });

            DevExpress.XtraReports.Web.ClientControls.LoggerService.Initialize((exception, message) =>
            {
                var logMessage = $"[{DateTime.Now}]: Exception occurred. Message: '{message}'. Exception Details:\r\n{exception}";
                ProcessException(exception, logMessage);
            });
            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;
            // DevExpressControls
            app.UseDevExpressControls();
            //DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new ReportStorageWebExtension());
            // devexpress Exceptions
            //DevExpress.XtraReports.Web.ClientControls.LoggerService.Initialize(ProcessException);


            //use gulp if it works https://docs.abp.io/en/abp/4.0/UI/AspNetCore/Client-Side-Package-Management
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),
            //    RequestPath = "/node_modules"
            //});


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<AbpCommonHub>("/signalr");
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("defaultWithArea", "{area}/{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger(c => { c.RouteTemplate = "swagger/{documentName}/swagger.json"; });

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                // specifying the Swagger JSON endpoint.
                options.SwaggerEndpoint($"/swagger/{_apiVersion}/swagger.json", $"Erp API {_apiVersion}");
                options.IndexStream = () => Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("Niva.Erp.Web.Host.wwwroot.swagger.ui.index.html");
                options.DisplayRequestDuration(); // Controls the display of the request duration (in milliseconds) for "Try it out" requests.  
            }); // URL: /swagger
        }

        //devexpress exception processing
        void ProcessException(Exception ex, string message)
        {
            // Log exceptions here. For instance:
            System.Diagnostics.Debug.WriteLine("[{0}]: Exception occured. Message: '{1}'. Exception Details:\r\n{2}",
                DateTime.Now, message, ex);
        }
    }
}
