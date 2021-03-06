using System;
using System.Threading.Tasks;
using BKabanApi.Models;
using BKabanApi.Models.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BKabanApi
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private readonly string _sqlConnectionString;

        public Startup(IConfiguration configuration)
        {
            _sqlConnectionString = configuration.GetConnectionString("BKabanConnectionString");
            _sqlConnectionString ??= configuration.GetConnectionString("localPC");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string myCORSOrigin = "https://kban.tk";

#if DEBUG
            myCORSOrigin = "http://localhost:8080";
#endif

            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(myCORSOrigin)
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowedToAllowWildcardSubdomains();
                    });
            });

            services.AddControllers();

            services.AddDistributedMemoryCache();
            services.AddSession(opt =>
            {
                opt.Cookie.IsEssential = true;
                opt.Cookie.HttpOnly = true;
                opt.Cookie.Name = ".BKaban.Session";
                opt.IdleTimeout = TimeSpan.FromMinutes(10);
                opt.Cookie.SameSite = SameSiteMode.None;
            });

            services.AddTransient<IUserRepository, UserRepository>(provider => new UserRepository(_sqlConnectionString));
            services.AddTransient<IBoardRepository, BoardRepository>(provider => new BoardRepository(_sqlConnectionString));
            services.AddTransient<IColumnRepository, ColumnRepository>(provider => new ColumnRepository(_sqlConnectionString));
            services.AddTransient<ITaskRepository, TaskRepository>(provider => new TaskRepository(_sqlConnectionString));
            services.AddTransient<IUserDataRepository, UserDataRepository>(provider => new UserDataRepository(_sqlConnectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(ctx =>
            {
                ctx.Response.Redirect("https://kban.tk", true);
                return Task.CompletedTask;
            });

        }
    }
}
