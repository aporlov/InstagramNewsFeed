using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using InstaSharper.API;
using InstaSharper.API.Builder;
using InstaSharper.Classes;
using InstaSharper.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace InstagramNewsFeed
{
    public class Startup
    {
        private static IInstaApi _instaApi;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = @"Server=(localdb)\mssqllocaldb;Database=Instagram;Trusted_Connection=True;ConnectRetryCount=0";
            services.AddDbContext<ApiDbContext>(options => options.UseSqlServer(connection));

            services.AddHangfire(x => x.UseSqlServerStorage(connection));

            //services.AddDbContext<ApiDbContext>(options =>
            //{
            //    // Use an in-memory database with a randomized database name (for testing)
            //    options.UseInMemoryDatabase("TestNewsList");
            //});

            services.AddMvc();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Instagram News Feed API with Swagger",
                    Version = "v1",
                    Description = " ",
                    TermsOfService = "None",
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                using (var serviceScope = app.ApplicationServices
                  .GetRequiredService<IServiceScopeFactory>()
                  .CreateScope())
                {
                    var dbContext = serviceScope.ServiceProvider.GetService<ApiDbContext>();
                    //AddTestData(dbContext);
                }
            }

                
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Instagram API V1");
            });

            // Enable Hangfire server
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            // create user session data and provide login details
            var userSession = new UserSessionData
            {
                UserName = Configuration["intsagramlogin"] ,
                Password = Configuration["instagrampass"]
            };
            RecurringJob.AddOrUpdate(
                () => GetNewsAsync(loggerFactory, userSession).Wait() , Cron.MinuteInterval(10));
            app.UseMvc();
        }
        public static async Task<bool> GetNewsAsync(ILoggerFactory loggerFactory, UserSessionData userSession)
        {
             ILogger _logger;
            _logger = loggerFactory.CreateLogger("Fetch&SaveNews");
            try
            {         
                _logger.LogInformation("Starting demo of InstaSharper project");
 
                // create new InstaApi instance using Builder
                _instaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(InstaSharper.Logger.LogLevel.All)) // use logger for requests and debug messages
                    .SetRequestDelay(TimeSpan.FromSeconds(2))
                    .Build();
                // login
                _logger.LogInformation($"Logging in as { userSession.UserName}");
                var logInResult = await _instaApi.LoginAsync();
                if (!logInResult.Succeeded)
                {
                    _logger.LogInformation($"Unable to login: {logInResult.Info.Message}");
                }
                else
                {
                    var userFeed = await _instaApi.GetUserTimelineFeedAsync(5); 

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error message {ex.Message}", ex.Message);
            }
            finally
            {
                var logoutResult = Task.Run(() => _instaApi.LogoutAsync()).GetAwaiter().GetResult();
                if (logoutResult.Succeeded) Console.WriteLine("Logout succeed");
            }
            return false;
        }
    }
   
}


