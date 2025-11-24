using catalog_safeway.Data;
using catalog_safeway.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // sql connection
builder.Services.AddScoped<IProductServices, ProductService>();
//session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// ✅ Set EPPlus license for non-commercial use
// Set EPPlus license
var license = new EPPlusLicense();
license.SetNonCommercialPersonal("Alejandro");





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Product/Main");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//session
app.UseSession();
//end session
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Main}/{id?}");

app.Run();
