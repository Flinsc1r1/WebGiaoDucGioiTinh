using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminNewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Trang danh sách bài viết
        public IActionResult Index()
        {
            var listNews = _context.FeaturedNewsList.OrderByDescending(n => n.CreatedDate).ToList();
            return View(listNews);
        }

        // 2. Giao diện Đăng bài viết mới
        public IActionResult Create()
        {
            return View();
        }

        // 3. Xử lý lưu bài viết mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FeaturedNews model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                _context.FeaturedNewsList.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"⚠️ LỖI DATABASE: {ex.Message}");
            }
        }

        // 4. Giao diện Chỉnh sửa bài viết
        public IActionResult Edit(int id)
        {
            var news = _context.FeaturedNewsList.FirstOrDefault(n => n.Id == id);
            if (news == null) return NotFound();
            return View(news);
        }

        // 5. Xử lý lưu sau khi Chỉnh sửa bài viết
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(FeaturedNews model)
        {
            try
            {
                var news = _context.FeaturedNewsList.FirstOrDefault(n => n.Id == model.Id);
                if (news != null)
                {
                    news.Title = model.Title;
                    news.Desc = model.Desc;
                    news.Tag = model.Tag;
                    news.Img = model.Img;
                    news.Content = model.Content;
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"⚠️ LỖI KHI SỬA: {ex.Message}");
            }
        }

        // 6. Xử lý Xóa bài viết (Chỉ để 1 hàm này thôi nhé!)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var news = _context.FeaturedNewsList.FirstOrDefault(n => n.Id == id);
                if (news != null)
                {
                    _context.FeaturedNewsList.Remove(news);
                    _context.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content($"⚠️ LỖI XÓA BÀI: {ex.Message}");
            }
        }
    }
}
