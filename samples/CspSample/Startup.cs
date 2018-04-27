using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Csp.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CspSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCsp(options =>
            {
                options.AddPolicy("MyForumPolicy", policy => policy
                    .AddDefaultSrc(src =>
                    {
                        src.AllowHost("http://*.example.com");
                        src.AllowSchema(CspDirectiveSchemas.Https);
                        src.AllowSchema("file:");
                        src.AllowSelf();
                        src.AllowUnsafeInline();
                        src.AllowEval();
                        src.AllowHash("sha256-qznLcsROx4GACP2dm0UCKCzCG+HiZ1guq6ZZDob/Tng=");
                        src.UseStrictDynamic();
                    })
                    .AddScriptSrc(src => src.AddNonce())
                    .AddStyleSrc(src => src.AddNonce())
                    .AddManifestSrc(src =>
                    {
                        src.AllowHost("http://*.example.com");
                    })
                    .ReportOnly()
                );

                options.AddPolicy("MyBlogPolicy", policy =>
                    policy.AddDefaultSrc(src => src.AllowSelf().AddNonce())
                );
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCsp("MyForumPolicy");
            app.UseCsp("MyBlogPolicy");
            app.UseCsp(policy =>
            {
                policy.AddDefaultSrc(src => src.AllowSelf());
                policy.AddScriptSrc(src => src.AllowUnsafeInline().AddNonce());
                policy.RequireSubresourceIntegrity(Subresource.Script, Subresource.Style);
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello from CSP sample!");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
