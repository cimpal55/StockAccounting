using LinqToDB;
using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using NToastNotify;
using StockAccounting.Core.Data.DbAccess;
using StockAccounting.Web.Utils.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews().AddNToastNotifyToastr(new ToastrOptions
{
    ProgressBar = true,
    TimeOut = 5000
});

builder.Services.AddLinqToDBContext<AppDataConnection>((provider, options) =>
{
    var connectionString = provider.GetRequiredService<IConfiguration>()
        .GetConnectionString("Default");

    return options
        .UseSqlServer(connectionString)
        .UseDefaultLogging(provider);
});

builder.Services.AddStockAccountingServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ExternalData}/{action=List}/{id?}/{name?}/");

app.Run();
