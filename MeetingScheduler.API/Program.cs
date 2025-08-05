using MeetingScheduler.Application.Services;
using MeetingScheduler.Infrastructure.Repositories;
using MeetingScheduler.Domain.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<MemoryRepository>();
builder.Services.AddSingleton<IHandleService, HandleService>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Meeting Handler API",
        Version = "v1",
        Description = "API for handling meetings"
    });
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meeting Handler API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();

app.Run();