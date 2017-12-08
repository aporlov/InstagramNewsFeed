using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApiDbContext>(options =>
            {
                // Use an in-memory database with a randomized database name (for testing)
                options.UseInMemoryDatabase("TestNewsList");
            });

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
                    AddTestData(dbContext);
                }
            }

                
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Instagram API V1");
            });

            app.UseMvc();
        }
        private static void AddTestData(ApiDbContext context)
        {
            // Test data

            var conv1 = context.NewsFeed.Add(new Models.NewsFeedEntity
            {
                Id = Guid.Parse("6f1e369b-29ce-4d43-b027-3756f03899a1"),
                CreatedAt = DateTimeOffset.UtcNow,
                Data = @"[ 
                     { 
                       ""id"":""1234567890123456789_1234567890"", 
                       ""user"": { ... }, 
                       ""images"": { 
                         ""thumbnail"": { ... }, 
                         ""low_resolution"": { ... }, 
                         ""standard_resolution"": { ... } 
                       }, 
                      ""created_time"": ""1234567890"", 
                       ""caption"": { 
                         ""id"":""12345678901234567890"", 
                         ""text"": ""My text"", 
                         ""created_time"": ""1234567890"", 
                         ""from"": { ... } 
                       }, 
                       ""user_has_liked"": false, 
                       ""likes"": { ... }, 
                       ""tags"": [ ... ], 
                       ""filter"": ""Normal"", 
                       ""comments"": { ... }, 
                       ""type"": ""image"", 
                       ""link"": ""https://www.instagram.com/p/12345abcdef/"", 
                       ""location"": null, 
                       ""attribution"": null, 
                       ""users_in_photo"": [ ... ] 
                       }
                ] 
                }"
            }).Entity;

            var conv2 = context.NewsFeed.Add(new Models.NewsFeedEntity
            {
                Id = Guid.Parse("2d555f8f-e2a2-461e-b756-1f6d0d254b46"),
                CreatedAt = DateTimeOffset.UtcNow,
                Data = @" [ 
                     { 
                       ""id"":""1234567890123456789_1234567890"", 
                       ""user"": { ... }, 
                       ""images"": { 
                         ""thumbnail"": { ... }, 
                         ""low_resolution"": { ... }, 
                         ""standard_resolution"": { ... } 
                       }, 
                      ""created_time"": ""1234567890"", 
                       ""caption"": { 
                         ""id"":""12345678901234567890"", 
                         ""text"": ""My text"", 
                         ""created_time"": ""1234567890"", 
                         ""from"": { ... } 
                       }, 
                       ""user_has_liked"": false, 
                       ""likes"": { ... }, 
                       ""tags"": [ ... ], 
                       ""filter"": ""Normal"", 
                       ""comments"": { ... }, 
                       ""type"": ""image"", 
                       ""link"": ""https://www.instagram.com/p/12345abcdef/"", 
                       ""location"": null, 
                       ""attribution"": null, 
                       ""users_in_photo"": [ ... ] 
                     } 
                ] "
            }).Entity;

            context.SaveChanges();

        }
    }
}
