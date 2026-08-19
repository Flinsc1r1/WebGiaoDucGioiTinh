using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

var builder = WebApplication.CreateBuilder(args);

// =================================================================================
// 🔥 CẤU HÌNH BỔ SUNG: Cho phép Server nhận bài viết chứa chuỗi hình ảnh lớn (Base64)
// =================================================================================
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 104857600; // Mở rộng tối đa 100 MB cho máy chủ Kestrel
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = 104857600; // Mở rộng tối đa 100 MB cho dữ liệu truyền tải từ Form
    options.MemoryBufferThreshold = int.MaxValue;
});
// =================================================================================

// 1. Thêm dịch vụ MVC & API
builder.Services.AddControllersWithViews();
builder.Services.AddControllers(); // Thêm dòng này để hỗ trợ API Chat
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// 2. Cấu hình Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Thiếu chuỗi kết nối 'DefaultConnection'.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. CẤU HÌNH IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.Cookie.Name = "WebGiaoDucGioiTinh_Auth";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
});

var app = builder.Build();

// 4. Khởi tạo Database và Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await SeedData.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi Seed Data.");
    }
}

// 5. Cấu hình HTTP Pipeline (Thứ tự cực kỳ quan trọng)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Đặt Session sau Routing
app.UseAuthentication(); // Auth trước
app.UseAuthorization();  // Author sau
app.MapStaticAssets();

// QUAN TRỌNG: Cấu hình cho API Chat AI
app.MapControllers();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");