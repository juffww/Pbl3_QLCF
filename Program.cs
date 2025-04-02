using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using pbl3_QLCF.Data;
using pbl3_QLCF.Models;
using pbl3_QLCF.Service;
//using pbl3_QLCF.Service;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Pbl3Context>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("CafeLink"));
});
builder.Services.AddSession();

// Thêm vào Program.cs hoặc Startup.cs
builder.Services.AddTransient<IMyEmailSender>(provider =>
    new MyEmailSender(
        "baoho1503@gmail.com",  // Thay bằng email của bạn
        "xbuy fprn swrn ilhu"          // Thay bằng password hoặc App Password
    ));

builder.Services.AddControllersWithViews();
// Các cấu hình khác...
builder.Services.AddControllersWithViews();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LoginAccess}/{action=Login}/{id?}");

app.Run();
