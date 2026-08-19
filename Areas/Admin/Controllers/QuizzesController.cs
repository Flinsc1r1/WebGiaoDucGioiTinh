using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Areas.Admin.ViewModels;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Areas.Admin.Controllers;

[Area("Admin")]
public class QuizzesController : Controller
{
    private readonly ApplicationDbContext _context;

    public QuizzesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Lesson)
            .OrderBy(q => q.Lesson!.Title)
            .ThenBy(q => q.Id)
            .Select(q => new QuizIndexRow(q.Id, q.Question, q.CorrectAnswer, q.Lesson!.Title))
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateLessonsDropDown();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Question,OptionA,OptionB,OptionC,OptionD,CorrectAnswer,LessonId")] Quiz quiz)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLessonsDropDown(quiz.LessonId);
            return View(quiz);
        }

        _context.Add(quiz);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã thêm câu hỏi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null) return NotFound();
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz is null) return NotFound();
        await PopulateLessonsDropDown(quiz.LessonId);
        return View(quiz);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Question,OptionA,OptionB,OptionC,OptionD,CorrectAnswer,LessonId")] Quiz quiz)
    {
        if (id != quiz.Id) return NotFound();
        if (!ModelState.IsValid)
        {
            await PopulateLessonsDropDown(quiz.LessonId);
            return View(quiz);
        }

        try
        {
            _context.Update(quiz);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Quizzes.AnyAsync(e => e.Id == quiz.Id))
                return NotFound();
            throw;
        }

        TempData["Success"] = "Đã cập nhật câu hỏi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null) return NotFound();
        var quiz = await _context.Quizzes
            .AsNoTracking()
            .Include(q => q.Lesson)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (quiz is null) return NotFound();
        return View(quiz);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz is null) return NotFound();

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Đã xóa câu hỏi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateLessonsDropDown(object? selectedId = null)
    {
        var items = await _context.Lessons.AsNoTracking().OrderBy(l => l.Title).ToListAsync();
        ViewBag.LessonId = new SelectList(items, "Id", "Title", selectedId);
    }
}
