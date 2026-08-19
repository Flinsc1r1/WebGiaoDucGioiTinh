using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebGiaoDucGioiTinh.Models;

/// <summary>Câu hỏi gửi ẩn danh qua hộp thư thầm kín (không lưu danh tính người gửi).</summary>
public class AnonymousQuestion
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập nội dung câu hỏi.")]
    [StringLength(4000, MinimumLength = 1, ErrorMessage = "Nội dung từ 1 đến 4000 ký tự.")]
    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public bool IsAnswered { get; set; }

    // Lưu câu trả lời của Admin
    [Column(TypeName = "nvarchar(max)")]
    public string? AnswerContent { get; set; }

    // MÃ BÍ MẬT (Ví dụ: RGJ8u) - Dòng này mới nè Thiên!
    public string? SecretCode { get; set; }
}

