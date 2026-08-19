using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WebGiaoDucGioiTinh.Models;

public class Lesson
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề không được để trống")]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;

    /// <summary>Nội dung dạng HTML.</summary>
    [Required(ErrorMessage = "Nội dung không được để trống")]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? VideoUrl { get; set; }

    // ĐÃ SỬA: Loại bỏ giới hạn StringLength(500) để thoải mái dán link ảnh từ mạng không lo bị quá ký tự
    [Column(TypeName = "nvarchar(max)")]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public ICollection<Quiz>? Quizzes { get; set; } = new List<Quiz>();

    [NotMapped]
    public IFormFile? ImageFile { get; set; }
}