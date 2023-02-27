﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Travel.Application.Common.Interfaces;
using Travel.Data.Contexts;

namespace Travel.Data
{
	public static class DependencyInjection
	{
        public static IServiceCollection AddInfrastructureData(this IServiceCollection services)
        {


            services.AddDbContext<ApplicationDbContext>(options => options
              .UseSqlite("Data Source=TravelTourDatabase.sqlite3"));


            services.AddDbContext<IApplicationDbContext, ApplicationDbContext>();

            return services;
        }
    }
}
