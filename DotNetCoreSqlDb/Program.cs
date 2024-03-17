using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetCoreSqlDb.Data;
var builder = WebApplication.CreateBuilder(args);

// Add database context and cache
builder.Services.AddDbContext<MyDatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")));

services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = Configuration.GetValue<string>("SampleInstance");
    options.ConfigurationOptions = new ConfigurationOptions()
    {
        EndPoints = { Configuration.GetValue<string>("RedisCache:Host"), Configuration.GetValue<int>("RedisCache:Port") },
        Password = Configuration.GetValue<string>("RedisCache:Password"),
        ConnectRetry = 5,
        ReconnectRetryPolicy = new LinearRetry(1500),
        Ssl = true,
        AbortOnConnectFail = false,
        ConnectTimeout = 5000,
        SyncTimeout = 5000,
        DefaultDatabase = 0
     };
});


// Add services to the container.
builder.Services.AddControllersWithViews();

// Add App Service logging
builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Todos}/{action=Index}/{id?}");

app.Run();
