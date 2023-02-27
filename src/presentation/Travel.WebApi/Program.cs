using Travel.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Travel.WebApi.Helpers;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc;
using Travel.WebApi.Filters;
using System.Configuration;
using Travel.Application;
using Travel.Shared;
using Travel.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddApplication();
        builder.Services.AddInfrastructureData();
        builder.Services.AddInfrastructureShared(builder.Configuration);

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllers(options =>
                options.Filters.Add(new ApiExceptionFilter()));

        builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true
            );

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => c.OperationFilter<SwaggerDefaultValues>());

        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        builder.Services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        builder.Services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
        });

        //SerilogConfiguration.Configure(builder.Logging);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            var provider = app.Services.GetService<IApiVersionDescriptionProvider>();
            app.UseSwaggerUI(c =>
            {
                foreach(var description in provider!.ApiVersionDescriptions)
                {
                    c.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }
            });
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}