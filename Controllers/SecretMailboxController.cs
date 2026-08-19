using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Controllers;

public class SecretMailboxController : Controller
{
    private readonly ApplicationDbContext _context;

    public SecretMailboxController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Hàm phụ để sinh mã ngẫu nhiên 5 ký tự
    private string GenerateRandomCode(int length = 5)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    // 1. TRANG GỬI CÂU HỎI (Có thêm tính năng tự dọn dẹp)
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // TỰ ĐỘNG DỌN DẸP: Xóa tin nhắn cũ hơn 1 tháng
        var oneMonthAgo = DateTime.Now.AddMonths(-1);
        var oldQuestions = _context.AnonymousQuestions.Where(q => q.CreatedAt < oneMonthAgo);

        if (oldQuestions.Any())
        {
            _context.AnonymousQuestions.RemoveRange(oldQuestions);
            await _context.SaveChangesAsync();
        }

        return View(new AnonymousQuestion());
    }

    // 2. XỬ LÝ GỬI CÂU HỎI
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index([Bind("Content")] AnonymousQuestion input)
    {
        if (!ModelState.IsValid)
            return View(input);

        // TẠO MÃ DUY NHẤT (Không bao giờ trùng)
        string randomCode;
        bool isDuplicate;
        do
        {
            randomCode = GenerateRandomCode(5);
            isDuplicate = await _context.AnonymousQuestions.AnyAsync(q => q.SecretCode == randomCode);
        } while (isDuplicate);

        var entity = new AnonymousQuestion
        {
            Content = input.Content.Trim(),
            SecretCode = randomCode, // Lưu mã ngẫu nhiên
            CreatedAt = DateTime.Now,
            IsAnswered = false,
        };

        _context.AnonymousQuestions.Add(entity);
        await _context.SaveChangesAsync();

        // LƯU MÃ VÀO SESSION DẠNG CHUỖI (String)
        HttpContext.Session.SetString("LatestCode", randomCode);

        TempData["SecretSent"] = true;
        TempData["YourCode"] = randomCode;

        return RedirectToAction(nameof(Index));
    }

    // 3. TRANG TRA CỨU (Tra cứu theo mã qcode thay vì ID số)
    [HttpGet]
    public async Task<IActionResult> LookUp(string? qcode)
    {
        if (string.IsNullOrEmpty(qcode))
        {
            return View();
        }

        var question = await _context.AnonymousQuestions
            .FirstOrDefaultAsync(q => q.SecretCode == qcode);

        if (question == null)
        {
            ViewBag.Error = "Không tìm thấy câu hỏi nào với mã số này.";
        }

        return View(question);
    }
}