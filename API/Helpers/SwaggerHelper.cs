using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace API.Helpers
{
    public static class SwaggerHelper
    {
        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "Shop SAM API",
                        Description = "Shop SAM over this API",
                        Version = "ToDo",
                        Contact = new OpenApiContact
                        {
                            Email = "Adriana.Faraci@bbzsogr.com",
                            Name = "GIBS - Gewerbliche Industriele Berufsfachschule Solothurn"
                        }
                    });
            });
        }
        public static void UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShopAPI");
            });
        }
    }
}