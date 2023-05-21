global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Diagnostics;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.ChangeTracking;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.IdentityModel.Tokens;
global using NBitcoin;
global using NBitcoin.RPC;
global using NetTopologySuite.Geometries;
global using Solidarity.Application.Common;
global using Solidarity.Application.Accounts;
global using Solidarity.Application.Accounts.Profiles;
global using Solidarity.Application.Authentication;
global using Solidarity.Application.Authentication.Password;
global using Solidarity.Application.Campaigns;
global using Solidarity.Application.Campaigns.Media;
global using Solidarity.Application.Campaigns.Funding;
global using Solidarity.Application.Campaigns.Validation;
global using Solidarity.Application.Campaigns.Allocation;
global using Solidarity.Application.Files;
global using Solidarity.Application.PaymentMethods;
global using Solidarity.Infrastructure.Identity;
global using Solidarity.Infrastructure.Persistence;
global using Solidarity.Infrastructure.Payment;
global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel.DataAnnotations;
global using System.IdentityModel.Tokens.Jwt;
global using System.Linq;
global using System.Net;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using System.Text;
global using Xunit;
global using System.Text.RegularExpressions;
global using Microsoft.Extensions.Diagnostics.HealthChecks;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Linq.Expressions;
global using Throw;
global using A11d.Module;
global using Mapster;

TypeAdapterConfig.GlobalSettings.Default.PreserveReference(true);

WebApplication.CreateBuilder(args)
	.Install<Solid>().Build()
	.Configure<Solid>().Run();

[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1050", Justification = "This is a marker class.")]
public class Solid { }