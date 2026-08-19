using System.ComponentModel.DataAnnotations;

namespace WebGiaoDucGioiTinh.Models;

public class UserQuizProgress
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = ""; // ID của người dùng
    
    [Required]
    public int LessonId { get; set; } // ID của bài học đã hoàn thành
    
    public DateTime CompletedAt { get; set; } = DateTime.Now;
}