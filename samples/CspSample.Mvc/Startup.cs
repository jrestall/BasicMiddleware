﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CspSample.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCsp(options =>
            {
                options.AddPolicy("Policy1", policy => policy
                    .AddDefaultSrc(src =>
                    {
                        src.AllowSchema(CspDirectiveSchemas.Http);
                        src.AllowSchema(CspDirectiveSchemas.Https);
                        src.AllowSelf();
                        src.AllowEval();
                        src.AllowHash("sha256-qznLcsROx4GACP2dm0UCKCzCG+HiZ1guq6ZZDob/Tng=");
                        src.UseStrictDynamic();
                    })
                    .AddScriptSrc(src => src.AddNonce())
                    .AddStyleSrc(src => src.AddNonce())
                    .ReportUri("/csp-reports")
                    .ReportTo("csp-reports")
                    .AddManifestSrc(src =>
                    {
                        src.AllowHost("http://*.example.com");
                    })
                    .ReportOnly()
                );

                options.AddPolicy("Policy2", policy =>
                    policy.AddDefaultSrc(src => src.AllowNone().AddNonce())
                );

                options.AddPolicy("SubresourceIntegrityPolicy", policy => policy
                    .RequireSubresourceIntegrity(Subresource.Script, Subresource.Style)
                    .AddScriptSrc(src =>
                    {
                        src.AllowSelf();
                        //src.AllowHost("https://code.jquery.com");
                        //src.AllowHost("https://ajax.aspnetcdn.com");
                        src.RequireSampleInReport();
                    })
                    .AddStyleSrc(src =>
                    {
                        src.AllowSelf();
                        //src.AllowHost("https://ajax.aspnetcdn.com");
                    })
                    .ReportUri("/csp-reports")
                );

                options.DefaultPolicyName = "Policy1";
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddCspReportMediaType();

            services.AddMvcCore().AddCsp();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "csp-reports",
                    template: "csp-reports",
                    defaults: new { controller = "Home", action = "CspReports" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Enable}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
