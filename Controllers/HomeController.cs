using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebGiaoDucGioiTinh.Data; // Thư mục chứa ApplicationDbContext
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Hàm này giúp nạp dữ liệu Database thật vào HomeController gốc
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hàm hiển thị trang chủ công khai ngoài giao diện chính
        public IActionResult Index()
        {
            if (_context.FeaturedNewsList == null)
            {
                ViewBag.FeaturedNews = new System.Collections.Generic.List<FeaturedNews>();
                return View();
            }

            // Đọc dữ liệu từ Database SQL gửi ra ngoài trang chủ
            ViewBag.FeaturedNews = _context.FeaturedNewsList.OrderByDescending(n => n.CreatedDate).ToList();
            return View();
        }

        // Hàm xem chi tiết bài báo
        public IActionResult Details(int id)
        {
            if (_context.FeaturedNewsList == null) return NotFound();

            var article = _context.FeaturedNewsList.FirstOrDefault(n => n.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }
    }
}