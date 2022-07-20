using Example.UserManagementService.Web;
using ServiceStack;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
string environment = builder.Configuration["Environment"];

builder.Configuration.AddJsonFile($"{environment}.appsettings.json");

if (environment == "Local" || environment == "Staging")
    builder.WebHost.UseUrls("http://::5000");

builder.WebHost.UseKestrel(options =>
{
    options.ListenLocalhost(15000);

    if (environment == "Local" || environment == "Staging")
        options.ListenAnyIP(5000);
});

var app = builder.Build();

app.UseServiceStack(new AppHost(environment));

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.Run();
