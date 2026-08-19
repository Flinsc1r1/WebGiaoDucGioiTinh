using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Thêm cái này để dùng được CountAsync()
using WebGiaoDucGioiTinh.Data;    // Thêm cái này để nhận diện ApplicationDbContext
using WebGiaoDucGioiTinh.Models;
using System.Linq;

namespace WebGiaoDucGioiTinh.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;

    // SỬA CHỖ NÀY: Phải truyền context vào đây thì mới dùng được
    public AccountController(
        UserManager<ApplicationUser> userManager, 
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    // --- ĐĂNG KÝ ---
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        }
        return View(model);
    }

    // --- ĐĂNG NHẬP ---
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded) return RedirectToAction("Index", "Home");
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
        }
        return View(model);
    }

    // --- ĐĂNG XUẤT ---
    [HttpPost] // Nên dùng Post cho Logout để bảo mật hơn
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // --- HỒ SƠ CÁ NHÂN (PROFILE) ---
    [HttpGet]
   public async Task<IActionResult> Profile()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return RedirectToAction("Login");

    // 1. Tính tiến độ (giữ nguyên logic cũ)
    var totalLessons = await _context.Lessons.CountAsync();
    var completedCount = await _context.UserQuizProgresses
        .Where(p => p.UserId == user.Id)
        .Select(p => p.LessonId)
        .Distinct()
        .CountAsync();

    ViewBag.Progress = totalLessons > 0 ? (completedCount * 100 / totalLessons) : 0;

    // 2. LẤY LỊCH SỬ NỘP BÀI (PHẦN MỚI)
    // Kết hợp bảng Progress với bảng Lessons để lấy tên bài học
    // Trong AccountController.cs -> Hàm Profile()
var history = await (from p in _context.UserQuizProgresses
                     join l in _context.Lessons on p.LessonId equals l.Id
                     where p.UserId == user.Id
                     orderby p.CompletedAt descending
                     select new {
                         LessonTitle = l.Title,
                         Date = p.CompletedAt
                     }).ToListAsync();

ViewBag.History = history; // PHẢI CÓ DÒNG NÀY THÌ VIEW MỚI THẤY DỮ LIỆU
    return View(user);
}
}