using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGiaoDucGioiTinh.Models
{
    public class FeaturedNews
    {
        public int Id { get; set; } // Dùng để định danh bài viết khi click xem chi tiết
        public string Title { get; set; } // Tiêu đề tin tức
        public string Desc { get; set; } // Mô tả ngắn hiển thị ngoài trang chủ
        public string Tag { get; set; } // Chuyên mục (Ví dụ: Kỹ năng, Tâm lý...)
        public string Img { get; set; } // Link ảnh đại diện (Admin có thể sửa đổi)
        public string? Url { get; set; } // Đường dẫn (để mặc định hoặc dùng điều hướng)
        public string Content { get; set; } // Nơi lưu toàn bộ nội dung bài báo gồm chữ, ảnh, video
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày đăng bài
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}