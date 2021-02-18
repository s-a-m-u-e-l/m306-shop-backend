using System.Linq;
using System.Text;
using API.Helpers;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Data.Context;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Features;
using MediatR;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", false)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            Environment = environment;
        }

        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<SamadDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DbConnection"));
            });
            //Swagger
            services.AddCustomSwagger();

            services.AddResponseCompression();


            services.AddControllers(x => x.EnableEndpointRouting = false).AddNewtonsoftJson();

            // MediatR
            var assembly = typeof(FeaturesModule).Assembly;
            services.AddMediatR(assembly);


            // Auto-mapping 
            services.AddAutoMapper(typeof(Startup));

            services.AddMvc();
            services.AddCors();

            services.AddDistributedMemoryCache();

            services.AddOData();

            services.AddHttpContextAccessor();

            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>()
                    .Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>()
                    .Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(
                        new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
            });
            
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            
            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = true;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(Configuration.GetSection("SECRET").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            // Register custom services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ILoginService, LoginService>();

            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IWishlistService, WishlistService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseCors(builder => { builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); });

            app.UseCustomSwagger();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Select().OrderBy().Filter().MaxTop(10000);
            });
        }
    }
}