using System;
using System.IO;
using BKabanApi.Models;
using BKabanApi.Models.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace BKabanApi
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            
            services.AddDistributedMemoryCache();
            services.AddSession(opt =>
            {
                opt.Cookie.IsEssential = true;
                opt.Cookie.HttpOnly = true;
                opt.Cookie.Name = ".BKaban.Session";
                opt.IdleTimeout = TimeSpan.FromMinutes(10);
            });

            string sqlConnectionString = "Server=DESKTOP-EQ1LBTR;Initial Catalog=BKabanDB;Integrated Security=True";
            services.AddTransient<IUserRepository, UserRepository>(provider => new UserRepository(sqlConnectionString));
            services.AddTransient<IFullBoardRepository, FullBoardRepository>(provider => new FullBoardRepository(sqlConnectionString));
            services.AddTransient<IColumnRepository, ColumnRepository>(provider => new ColumnRepository(sqlConnectionString));
            services.AddTransient<ITaskRepository, TaskRepository>(provider => new TaskRepository(sqlConnectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async ctx => await ctx.Response.SendFileAsync(@"wwwroot/index.html"));

        }
    }
}
