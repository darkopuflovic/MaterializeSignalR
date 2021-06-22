using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace MaterializeSignalR
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSession();
            services.AddMvc();
            services.AddSignalR();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "MaterializeChatCredentials";
                    options.ExpireTimeSpan = TimeSpan.FromDays(30);
                    options.SlidingExpiration = true;
                });
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("Keys"))
                .SetApplicationName("MaterializeChat")
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseSession();

            app.UseAuthentication();

            app.UseEndpoints(routes =>
            {
                routes.MapHub<ChatHub>("/ChatHub");
                routes.MapRazorPages();
            });
        }
    }
}
