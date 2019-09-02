using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Controllers;
using CoreCodeCamp.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions; //had to manually add
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CoreCodeCamp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CampContext>();
            services.AddScoped<ICampRepository, CampRepository>();

            //from: https://github.com/AutoMapper/AutoMapper.Extensions.Microsoft.DependencyInjection/issues/105
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            //here I have added versioning of all kinds of flavors in order to have a full example of
            //what asp net is capable of, of course in a real project we expect some standards (scinic wink)
            services.AddApiVersioning(opt =>
            {
                //attribute approach
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                opt.ReportApiVersions = true;
                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("HeadVer"),
                    new QueryStringApiVersionReader("UrlVer", "MyOtherParamThatWillBeRead"),
                    new UrlSegmentApiVersionReader()
                );

                //convention approach
                opt.Conventions.Controller<Camps2Controller>()
                    .HasApiVersion(new ApiVersion(3, 0))
                    .HasApiVersion(new ApiVersion(3, 1))
                    .Action(c => c.DeleteCamp(default(string)))
                    .MapToApiVersion(3, 1);
            }
            );

            //there is versioning by namespaces...:
            //also versioning for content type
            //writing my own reader
            //also, writting my own resolvers

            services.AddMvc(opt =>
            {
                //opt.EnableEndpointRouting = false;
            })
              .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
