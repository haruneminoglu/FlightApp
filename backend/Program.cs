using Microsoft.EntityFrameworkCore;
using FlightApp.Data; // ApplicationDbContext sınıfının bulunduğu namespace'i ekleyin
using Microsoft.AspNetCore.Authentication.Cookies;
using Grpc.Net.Client;
using FlightApp.Services; // PassengerGrpcClient'ın bulunduğu namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<PassengerGrpcClient>();

builder.Services.AddGrpcClient<PassengerGrpcService.PassengerService.PassengerServiceClient>(o =>
{
    var address = builder.Configuration["GrpcSettings:PassengerServiceUrl"];
    if (string.IsNullOrEmpty(address))
        throw new ArgumentNullException(nameof(address), "GrpcSettings:PassengerServiceUrl is not configured");
    o.Address = new Uri(address);
});

// Veritabanı bağlantısını yapılandır
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 40)) // MySQL sunucusunun sürümüne uygun olacak şekilde güncelledik
    )
);

// Cookie Authentication'ı ekle
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Oturum hizmetlerini ekleyin
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// IHttpContextAccessor'u ekleyin
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Authorization için rol bazlı politika ekleme
builder.Services.AddAuthorization(options =>
{
    // Adminler için özel erişim
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    // Normal kullanıcılar için özel erişim
    options.AddPolicy("User", policy => policy.RequireRole("User"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Geliştirme ortamında ayrıntılı hata bilgilerini göster
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Middleware sırasına dikkat edin
app.UseSession(); // Oturum middleware'ini kullan
app.UseAuthentication(); // Authentication middleware'ini ekle
app.UseAuthorization(); // Authorization middleware'ini ekle

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Admin sayfaları için route ekleyelim
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=AdminFlights}/{action=Index}/{id?}");

app.Run();
