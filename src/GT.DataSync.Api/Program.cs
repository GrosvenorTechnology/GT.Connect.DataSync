using GT.DataSync.Api.Services;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
     .AddXmlSerializerFormatters()
     .AddJsonOptions(options =>
               options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.EnableAnnotations();
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ControllerCommandAPI", Version = "v1" });

    opt.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "0.00:00:00", Reference = null, Nullable = false });
    opt.MapType<TimeSpan?>(() => new OpenApiSchema { Type = "string", Format = "0.00:00:00", Reference = null, Nullable = true });

    opt.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY", //header with api key
        Type = SecuritySchemeType.ApiKey,
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
             {
                 {
                     new OpenApiSecurityScheme
                     {
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                     },
                     new string[] {}
                 }
             });

});

builder.Services.AddSingleton<TrackedSampleDataService>();
builder.Services.AddSingleton<AuthoritativeSampleDataService>();
builder.Services.AddSingleton<PagedSampleDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}

app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Custom Exchange Data Sync API V1");
    x.SupportedSubmitMethods(new[] { SubmitMethod.Get, SubmitMethod.Post });
    x.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//This enables buffering so we can dump the content of the 
// Request to the log, not needed in production
app.Use(next => context => {
    context.Request.EnableBuffering();
    return next(context);
});

app.Run();
