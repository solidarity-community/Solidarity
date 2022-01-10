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
global using Solidarity.Domain.Enums;
global using Solidarity.Domain.Exceptions;
global using Solidarity.Domain.Extensions;
global using Solidarity.Domain.Models;
global using Solidarity.Infrastructure.Crypto;
global using Solidarity.Infrastructure.Identity;
global using Solidarity.Infrastructure.Persistence;
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
global using Xunit;

WebApplication.CreateBuilder(args)
	.ConfigureServices().Build()
	.ConfigureApplication().Run();

public static class ConfigurationExtensions
{
	public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddHttpContextAccessor();
		builder.Services.AddCors();
		builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
		builder.Services.AddMvc();
		builder.Services.InstallSolidarity();
		return builder;
	}

	private static void InstallSolidarity(this IServiceCollection services)
	{
		new AuthenticationInstaller().Install(services);
		new CryptoClientInstaller().Install(services);
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