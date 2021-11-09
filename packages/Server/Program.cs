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
global using Microsoft.Extensions.Configuration;
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
global using Solidarity.Infrastructure.Persistance;
global using Solidarity.Installers;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.IO.Ports;
global using System.Linq;
global using System.Linq.Expressions;
global using System.Net;
global using System.Runtime.InteropServices;
global using System.Security.Authentication;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using System.Threading.Tasks;
global using Xunit;

namespace Solidarity;

public class Program
{
	public static IConfiguration? Configuration { get; set; }

	public static void Main(string[] args) =>
		Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>()).Build().Run();
}