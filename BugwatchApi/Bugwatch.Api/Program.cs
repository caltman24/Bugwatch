using Bugwatch.Api.Extensions;
using Bugwatch.Api.Filters;
using Bugwatch.Api.Modules;
using Bugwatch.Application;
using Bugwatch.Application.Constants;
using Bugwatch.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddResponseCompressionService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProjectModule()
    .UseTeamModule()
    .UseTicketModule();

app.MapGet("/test/rolefilter", ([FromQuery] string authId) => Results.Ok("Test endpoint"))
    .WithMemberRole(UserRoles.Developer, UserRoles.Admin, UserRoles.ProjectManager);

app.UseResponseCompression();
app.Run();