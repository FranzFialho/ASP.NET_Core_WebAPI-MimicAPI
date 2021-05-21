using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Helpers.Swagger;
using MimicAPI.Repositories.v1.Contracts;
using MimicAPI.v1.Repositories;
using System.IO;
using System.Linq;

namespace MimicAPI
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
            #region AutoMapper - Config
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DTOMapperProfile());
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #endregion

            services.AddDbContext<MimicContext>(opt =>
            {
                opt.UseSqlite("Data Source=Database\\Mimic.db");
            });

            services.AddMvc();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<IPalavraRepository, PalavraRepository>();
            services.AddApiVersioning(cfg =>
            {
                cfg.ReportApiVersions = true;

                //cfg.ApiVersionReader = new HeaderApiVersionReader("api-version");
                cfg.AssumeDefaultVersionWhenUnspecified = true;
                cfg.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.ResolveConflictingActions(apiDescription => apiDescription.First());

                cfg.SwaggerDoc("v2.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V2.0",
                    Version = "v2.0"
                });

                cfg.SwaggerDoc("v1.1", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V1.1",
                    Version = "v1.1"
                });

                cfg.SwaggerDoc("v1.0", new Swashbuckle.AspNetCore.Swagger.Info()
                {
                    Title = "MimicAPI - V1.0",
                    Version = "v1.0"
                });

                var CaminhoProjeto = PlatformServices.Default.Application.ApplicationBasePath;
                var NomeProjeto = $"{PlatformServices.Default.Application.ApplicationName}.xml";
                var CamimhoArquivoXMLComentario = Path.Combine(CaminhoProjeto, NomeProjeto);

                cfg.IncludeXmlComments(CamimhoArquivoXMLComentario);

                cfg.DocInclusionPredicate((docName, apiDesc) =>
                {
                    var actionApiVersionModel = apiDesc.ActionDescriptor?.GetApiVersion();
                // would mean this action is unversioned and should be included everywhere
                if (actionApiVersionModel == null)
                    {
                        return true;
                    }
                    if (actionApiVersionModel.DeclaredApiVersions.Any())
                    {
                        return actionApiVersionModel.DeclaredApiVersions.Any(v => $"v{v.ToString()}" == docName);
                    }
                    return actionApiVersionModel.ImplementedApiVersions.Any(v => $"v{v.ToString()}" == docName);
                });

                cfg.OperationFilter<ApiVersionOperationFilter>();
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("/swagger/v2.0/swagger.json", "MimicAPI - V2.0");
                cfg.SwaggerEndpoint("/swagger/v1.1/swagger.json", "MimicAPI - V1.1");
                cfg.SwaggerEndpoint("/swagger/v1.0/swagger.json", "MimicAPI - V1.0");
                cfg.RoutePrefix = string.Empty;
            });
        }
    }
}
