﻿using Domain.Repositories;
using InnoClinic.ProfilesAPI.Converters;
using InnoClinic.ProfilesAPI.Middleware.Exception_Handler;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Persistence;
using Services;
using Services.Abstractions;
using System.Security.Claims;

namespace InnoClinic.ProfilesAPI.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
                });
        }
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "ProfilesAPI", Version = "v1" });
                options.MapType<DateOnly>(() => new OpenApiSchema
                {
                    Type = "string",
                    Format = "date",
                    Example = new OpenApiString("2022-01-01")
                });
            });
        }
        public static void ConfigureEntityServices(this IServiceCollection services)
        {
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddScoped<IRepositoryManager, RepositoryManager>();
            services.AddScoped<IValidatorManager, ValidatorManager>();
        }
        public static void ConfigureMsSqlServerContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<RepositoryDbContext>(dbcontextBuilder =>
            {
                var connectionString = configuration.GetConnectionString("ProfilesDb");
                dbcontextBuilder.UseSqlServer(connectionString);
            });

        }
        public static void ConfigureAutomapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Services.AssemblyReference).Assembly);
        }
        public static void CofigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", opt =>
               {
                   opt.RequireHttpsMetadata = false;
                   opt.Authority = "https://localhost:5005";
                   opt.Audience = "profiles";
               });
        }
        public static void ConfigureOwnerAuthZPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("OwnerOrReceptionist", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var userId = context.User.FindFirstValue("sub");
                        var routeId = new HttpContextAccessor().HttpContext.Request.RouteValues["userid"].ToString();
                        return userId == routeId || context.User.IsInRole("Receptionist");
                    });
                });
                opts.AddPolicy("OwnerOrDoctorOrReceptionist", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        var userId = context.User.FindFirstValue("sub");
                        var routeId = new HttpContextAccessor().HttpContext.Request.RouteValues["userid"].ToString();
                        return userId == routeId || context.User.IsInRole("Receptionist") || context.User.IsInRole("Doctor");
                    });
                });

            });
        }

        /* --- CUSTOM MIDDLEWARE --- */
        public static void UseExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionHandlerMiddleware>();
        }

    }
}
