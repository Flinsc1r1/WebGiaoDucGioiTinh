using Microsoft.AspNetCore.Identity;

namespace WebGiaoDucGioiTinh.Models
{
    public class ApplicationUser : IdentityUser
    {
        // ĐƯA 2 DÒNG NÀY VÀO TRONG ĐÂY 👇
        public string CurrentRank { get; set; } = "Đồng V"; // Hạng hiện tại
        public int CurrentStars { get; set; } = 0;           // Số sao đang có

        // Thêm các trường tùy chỉnh cho phù hợp với web giáo dục
        public string? FullName { get; set; }
        public string? AvatarUrl { get; set; }
        public int Points { get; set; } = 0; // Điểm tích lũy khi làm Quiz
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}