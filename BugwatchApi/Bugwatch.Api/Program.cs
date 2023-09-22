using Bugwatch.Api.Extensions;
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

// TODO: REMOVE all authId queryStrings from all endpoints. Migrate to Authorization
builder.Services.AddAuthorization(opts =>
{
    opts.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireClaim("sub")
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
    .UseTicketModule();

app.UseResponseCompression();
app.Run();