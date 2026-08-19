using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Areas.Admin.ViewModels;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoriesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
{

    var list = await _context.Categories
        .AsNoTracking()
        .OrderBy(c => c.Name)
        .Select(c => new CategoryIndexRow(c.Id, c.Name, c.Lessons.Count))
        .ToListAsync();
        
    return View(list);
}

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name")] Category category)
    {
        if (!ModelState.IsValid)
            return View(category);

        _context.Add(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm danh mục.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
       if (id is null) return NotFound();
    // Thêm Include để lấy số bài
    var category = await _context.Categories.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == id);
    if (category is null) return NotFound();
    
    // Gửi số lượng qua ViewBag
    ViewBag.LessonCount = category.Lessons.Count;
    return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Category category)
    {
        if (id != category.Id) return NotFound();
        if (!ModelState.IsValid) return View(category);

        try
        {
            _context.Update(category);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Categories.AnyAsync(e => e.Id == category.Id))
                return NotFound();
            throw;
        }

        TempData["Success"] = "Đã cập nhật danh mục.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
        if (category is null) return NotFound();

        var count = await _context.Lessons.CountAsync(l => l.CategoryId == id);
        ViewBag.LessonCount = count;
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.Categories
            .Include(c => c.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return NotFound();

        if (category.Lessons.Count > 0)
        {
            TempData["Error"] = "Không xóa được: danh mục còn bài học. Hãy xóa hoặc chuyển bài học trước.";
            return RedirectToAction(nameof(Index));
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa danh mục.";
        return RedirectToAction(nameof(Index));
    }
}
