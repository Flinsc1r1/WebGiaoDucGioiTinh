using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Data;

namespace WebGiaoDucGioiTinh.Areas.Admin.Controllers;

[Area("Admin")]
public class AnonymousQuestionsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AnonymousQuestionsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _context.AnonymousQuestions
            .AsNoTracking()
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
        return View(list);
    }

[HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string answerContent)
    {
        var question = await _context.AnonymousQuestions.FindAsync(id);
        if (question == null) return NotFound();

        if (!string.IsNullOrWhiteSpace(answerContent))
        {
            question.AnswerContent = answerContent;
            question.IsAnswered = true;

            _context.Update(question);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}