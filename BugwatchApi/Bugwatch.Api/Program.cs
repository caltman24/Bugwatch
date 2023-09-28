using System.Security.Claims;
using Bugwatch.Api.Extensions;
using Bugwatch.Api.Helpers;
using Bugwatch.Api.Modules;
using Bugwatch.Application;
using Bugwatch.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// TODO: Add custom domain errors as an HTTP Problem Response
builder.Services.AddAuthorization(opts =>
{
    opts.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim(ClaimTypes.NameIdentifier)
        .Build();
});

builder.Services.AddResponseCompressionService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerOptions();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProjectModule()
    .UseTeamModule()
    .UseTicketModule()
    .UseAdminModule();

app.MapGet("/test/authid", ContextHelper.GetNameIdentifier).RequireAuthorization();

app.UseResponseCompression();
app.Run();