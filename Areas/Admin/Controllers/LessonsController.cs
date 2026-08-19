using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Areas.Admin.ViewModels;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Areas.Admin.Controllers;

[Area("Admin")]
public class LessonsController : Controller
{
    private readonly ApplicationDbContext _context;

    public LessonsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.Lessons
            .AsNoTracking()
            .Include(l => l.Category)
            .OrderBy(l => l.Category!.Name)
            .ThenBy(l => l.Title)
            .Select(l => new LessonIndexRow(l.Id, l.Title, l.Category!.Name, l.Quizzes.Count))
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesDropDown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lesson lesson)
    {
        // Loại bỏ kiểm tra các thuộc tính liên kết để tránh lỗi dữ liệu đầu vào ẩn
        ModelState.Remove("Category");
        ModelState.Remove("Quizzes");
        ModelState.Remove("ImageFile");

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesDropDown(lesson.CategoryId);
            return View(lesson);
        }

        // Nhận trực tiếp link ảnh bạn nhập/dán từ form
        _context.Add(lesson);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm bài học mới thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson is null) return NotFound();
        await PopulateCategoriesDropDown(lesson.CategoryId);
        return View(lesson);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Lesson lesson)
    {
        if (id != lesson.Id) return NotFound();

        // Loại bỏ kiểm tra các thuộc tính liên kết để tránh lỗi Validate oan
        ModelState.Remove("Category");
        ModelState.Remove("Quizzes");
        ModelState.Remove("ImageFile");

        if (ModelState.IsValid)
        {
            try
            {
                // Tìm bài học gốc trong Database để chỉnh sửa trực tiếp, tránh xung đột thực thể (Tracking)
                var lessonInDb = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
                if (lessonInDb == null) return NotFound();

                // Cập nhật các thông tin dạng chữ text
                lessonInDb.Title = lesson.Title;
                lessonInDb.Content = lesson.Content;
                lessonInDb.CategoryId = lesson.CategoryId;
                lessonInDb.VideoUrl = FixYouTubeUrl(lesson.VideoUrl);

                // Nhận link ảnh Google dán trực tiếp từ ô Textbox
                lessonInDb.ImageUrl = lesson.ImageUrl;

                // Lưu thay đổi vào DB, hoàn toàn không chạm vào ổ đĩa vật lý của máy
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã cập nhật bài học bằng link ảnh thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Lessons.AnyAsync(e => e.Id == lesson.Id)) return NotFound();
                throw;
            }
        }
        await PopulateCategoriesDropDown(lesson.CategoryId);
        return View(lesson);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var lesson = await _context.Lessons
            .AsNoTracking()
            .Include(l => l.Category)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (lesson is null) return NotFound();

        var qCount = await _context.Quizzes.CountAsync(q => q.LessonId == id);
        ViewBag.QuizCount = qCount;
        return View(lesson);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var lesson = await _context.Lessons.FindAsync(id);
        if (lesson is null) return NotFound();

        _context.Lessons.Remove(lesson);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa bài học thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesDropDown(object? selectedId = null)
    {
        var items = await _context.Categories.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
        ViewBag.CategoryId = new SelectList(items, "Id", "Name", selectedId);
    }

    private string? FixYouTubeUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return url;
        if (url.Contains("/embed/")) return url;

        var videoId = "";
        if (url.Contains("v="))
        {
            var parts = url.Split("v=")[1].Split('&');
            videoId = parts[0];
        }
        else if (url.Contains("youtu.be/"))
        {
            var parts = url.Split("youtu.be/")[1].Split('?');
            videoId = parts[0];
        }

        return !string.IsNullOrEmpty(videoId) ? $"https://www.youtube.com/embed/{videoId}" : url;
    }
}