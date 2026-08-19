namespace WebGiaoDucGioiTinh.Models;

public class TakeQuizViewModel
{
    public int LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public IReadOnlyList<QuizDisplayItem> Questions { get; set; } = Array.Empty<QuizDisplayItem>();

    /// <summary>JSON mảng { id, correct } dùng cho chấm điểm phía client.</summary>
    public string AnswersJson { get; set; } = "[]";
}

public class QuizDisplayItem
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
}
