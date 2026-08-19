using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Data;
namespace WebGiaoDucGioiTinh.Controllers;

public class LessonsController : Controller
{

    private readonly ApplicationDbContext _context;



    public LessonsController(ApplicationDbContext context)

    {

        _context = context;

    }



    public async Task<IActionResult> Index()

    {

        var categories = await _context.Categories

        .AsNoTracking()

        .Include(c => c.Lessons.OrderBy(l => l.Id))

        .OrderBy(c => c.Id)

        .ToListAsync();



        return View(categories);

    }



    public async Task<IActionResult> Details(int? id)

    {

        if (id is null)

            return NotFound();



        var lesson = await _context.Lessons

        .AsNoTracking()

        .Include(l => l.Category)

        .Include(l => l.Quizzes.OrderBy(q => q.Id))

        .FirstOrDefaultAsync(m => m.Id == id);



        if (lesson is null)

            return NotFound();



        return View(lesson);

    }
}