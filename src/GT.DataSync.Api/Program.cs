using GT.DataSync.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
     .AddXmlSerializerFormatters()
     .AddJsonOptions(options =>
               options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);


builder.Services.AddSingleton<AuthoritativeSampleDataService>();
builder.Services.AddSingleton<PagedSampleDataService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //This enables buffering so we can dump the content of the 
    // Request to the log, not needed in production
    app.Use(next => context => {
        context.Request.EnableBuffering();
        return next(context);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
