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
using Serilog;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using System.Xml.Linq;
using System.Reflection;
using Travel.Identity.Helpers;
using Travel.Identity;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddApplication();
        builder.Services.AddInfrastructureData();
        builder.Services.AddInfrastructureShared(builder.Configuration);
        builder.Services.AddInfrastructureIdentity(builder.Configuration);
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddControllers(options =>
                options.Filters.Add(new ApiExceptionFilter()));

        builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true
            );

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.OperationFilter<SwaggerDefaultValues>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        }, new List<string>()
                    }
                });
        });

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
            options.SubstituteApiVersionInUrl = true;
        });

        builder.Host.UseSerilog((hostContext, services, configuration) =>
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            configuration.MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .Enrich.WithMachineName()
                .Enrich.WithProperty("Assembly", $"{name.Name}")
                .Enrich.WithProperty("Assembly", $"{name.Version}")
                //.WriteTo.SQLite(sqliteDbPath: Environment.CurrentDirectory + @"/Logs/log.db",
                //    restrictedToMinimumLevel: LogEventLevel.Information,
                //    storeTimestampInUtc: true,
                //    batchSize: 1)
                .WriteTo.File(
                        new CompactJsonFormatter(),
                        Environment.CurrentDirectory + @"/Logs/log.json",
                        rollingInterval: RollingInterval.Day,
                        restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.Console();
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
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
        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}