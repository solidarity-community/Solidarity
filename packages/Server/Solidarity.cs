global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.EntityFrameworkCore.Metadata;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using NBitcoin;
global using NBitcoin.RPC;
global using NetTopologySuite.Geometries;
global using Solidarity.API.Extensions;
global using Solidarity.Application.Common;
global using Solidarity.Application.Extensions;
global using Solidarity.Application.Helpers;
global using Solidarity.Application.Services;
global using Solidarity.Core.Application;
global using Solidarity.Domain.Exceptions;
global using Solidarity.Domain.Extensions;
global using Solidarity.Domain.Models;
global using Solidarity.Infrastructure.Identity;
global using Solidarity.Infrastructure.Persistence;
global using Solidarity.Infrastructure.Payment;
global using Solidarity.Installers;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Net;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Reflection;
global using Xunit;
global using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NetTopologySuite.IO;

WebApplication.CreateBuilder(args)
	.ConfigureServices().Build()
	.ConfigureApplication().Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050:Declare types in namespaces", Justification = "This is a one-time extension method to use within top-level statements")]
public static class ConfigurationExtensions
{
	public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddCors();
		builder.Services.AddControllers().AddNewtonsoftJson(options =>
		{
			options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
			options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
			var geometryFactory = new GeometryFactoryEx(precisionModel: new PrecisionModel(), srid: 4326) { OrientationOfExteriorRing = LinearRingOrientation.CCW };
			var geographyConverters = GeoJsonSerializer.Create(geometryFactory).Converters;
			foreach (var converter in geographyConverters)
			{
				options.SerializerSettings.Converters.Add(converter);
			}
		});
		var validator = new SuppressChildValidationMetadataProvider(typeof(Geometry));
		builder.Services.AddMvc(options => options.ModelMetadataDetailsProviders.Add(validator));
		builder.Services.InstallSolidarity();
		return builder;
	}

	private static void InstallSolidarity(this IServiceCollection services)
	{
		new AuthenticationInstaller().Install(services);
		new PaymentMethodsInstaller().Install(services);
		new DatabaseInstaller().Install(services);
		new OpenApiInstaller().Install(services);
		new UserServiceInstaller().Install(services);
		new ServiceInstaller().Install(services);
	}

	public static WebApplication ConfigureApplication(this WebApplication application)
	{
		if (application.Environment.IsDevelopment())
		{
			application.UseDeveloperExceptionPage();
		}
		application.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
		application.UseHttpsRedirection();
		application.UseRouting();
		application.UseAuthentication();
		application.UseAuthorization();
		application.ConfigureExceptionHandler();
		application.UseEndpoints(endpoints => endpoints.MapControllers());
		application.UseSwagger();
		return application;
	}
}