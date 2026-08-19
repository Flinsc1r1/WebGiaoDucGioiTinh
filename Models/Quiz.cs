using System.ComponentModel.DataAnnotations;

namespace WebGiaoDucGioiTinh.Models;

public class Quiz
{
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Question { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string OptionA { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string OptionB { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string OptionC { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string OptionD { get; set; } = string.Empty;

    /// <summary>Giá trị: A, B, C hoặc D.</summary>
    [Required]
    [StringLength(1)]
    [RegularExpression(@"^[ABCD]$")]
    public string CorrectAnswer { get; set; } = string.Empty;

    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
}
