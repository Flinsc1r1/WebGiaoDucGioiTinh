namespace WebGiaoDucGioiTinh.Areas.Admin.ViewModels;

public record CategoryIndexRow(int Id, string Name, int LessonCount);

public record LessonIndexRow(int Id, string Title, string CategoryName, int QuizCount);

public record QuizIndexRow(int Id, string Question, string CorrectAnswer, string LessonTitle);
