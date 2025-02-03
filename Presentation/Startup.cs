using Application.Behaviors;
using Application.ClinicalTrialMetadata.Queries.GetClinicalTrialMetadataById;
using Domain.Abstractions;
using FluentValidation;
using Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Presentation.DTOs;
using Presentation.Filters;
using Presentation.Mapper;
using Presentation.Middleware;
using Presentation.Settings;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace Presentation;

public class Startup
{
    private object context;

    public Startup(IConfiguration configuration) => Configuration = configuration;

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure(Configuration);

        services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        var applicationAssembly = typeof(Application.AssemblyReference).Assembly;

        services.AddMediatR(applicationAssembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(applicationAssembly);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Presentation", Version = "v1" });
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, @"Presentation.xml"));
        });

        services.AddTransient<ExceptionHandlingMiddleware>();

        services.AddScoped<IJsonSchemaValidator, JsonSchemaValidator>();

        services.Configure<FileUploadSettings>(Configuration.GetSection("FileUploadSettings"));
        services.AddScoped<ValidateFileUploadFilter>();

        services.AddScoped<IMapper<ClinicalTrialMetadataDto, ClinicalTrialMetadataResponse>, ClinicalTrialMetadataMapper>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web v1"));
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
