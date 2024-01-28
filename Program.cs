using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
var myappOrigins = "_myAppOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(myappOrigins, policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor |
        Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

builder.Services.AddControllers();
builder.Services.AddOcelot();

var app = builder.Build();
app.UseForwardedHeaders();
app.UseCors(myappOrigins);
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseOcelot().Wait();
app.Run();