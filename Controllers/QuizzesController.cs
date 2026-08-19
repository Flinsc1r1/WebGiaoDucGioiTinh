using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Data;
using WebGiaoDucGioiTinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebGiaoDucGioiTinh.Controllers
{
    public class QuizzesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // Hàm khởi tạo Constructor để Inject Database và UserManager
        public QuizzesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. GIAO DIỆN CHÍNH CỦA TRÒ CHƠI
        public IActionResult GeneralQuiz()
        {
            return View();
        }

        // 1b. GIAO DIỆN LÀM BÀI QUIZ THEO BÀI HỌC (Sửa lỗi 404)
        public async Task<IActionResult> TakeQuiz(int id)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
            {
                return NotFound("Không tìm thấy bài học tương ứng.");
            }

            // Trả về View làm bài Quiz chuyên dụng cho bài học
            return View(lesson);
        }
        // 2. API LẤY NGẪU NHIÊN GÓI CÂU HỎI (Tối đa 30 câu)
        [HttpGet]
        public async Task<IActionResult> GetRandomQuestions()
        {
            if (_context.Quizzes == null)
            {
                return NotFound("Cơ sở dữ liệu câu hỏi trống.");
            }

            // Lấy ngẫu nhiên câu hỏi từ bảng Quizzes trong Database
            var randomQuestions = await _context.Quizzes
                .OrderBy(q => Guid.NewGuid())
                .Take(30)
                .Select(q => new
                {
                    id = q.Id,
                    question = q.Question,
                    optionA = q.OptionA,
                    optionB = q.OptionB,
                    optionC = q.OptionC,
                    optionD = q.OptionD
                })
                .ToListAsync();

            return Ok(randomQuestions);
        }

        // 3. API KIỂM TRA ĐÁP ÁN KHI NGƯỜI CHƠI BẤM CHỌN
        [HttpPost]
        public async Task<IActionResult> CheckAnswer([FromBody] AnswerCheckModel model)
        {
            var question = await _context.Quizzes.FindAsync(model.QuestionId);
            if (question == null) return NotFound("Không tìm thấy câu hỏi.");

            // So sánh đáp án người dùng chọn với đáp án đúng trong DB (Ví dụ: "A", "B", "C", "D")
            bool isCorrect = string.Equals(question.CorrectAnswer?.Trim(), model.SelectedAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);

            return Ok(new { isCorrect = isCorrect });
        }

        // 4. API TÍNH TOÁN CỘNG/TRỪ SAO VÀ ĐỔI RANK SAU KHI KẾT THÚC TRẬN
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateRankAfterMatch([FromBody] MatchResultModel model)
        {
            // Lấy thông tin tài khoản đang chơi
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Danh sách cấu trúc các bậc rank từ thấp đến cao
            var rankList = new List<string> {
                "Đồng V", "Đồng IV", "Đồng III", "Đồng II", "Đồng I",
                "Bạc V", "Bạc IV", "Bạc III", "Bạc II", "Bạc I",
                "Vàng V", "Vàng IV", "Vàng III", "Vàng II", "Vàng I",
                "Bạch Kim V", "Bạch Kim IV", "Bạch Kim III", "Bạch Kim II", "Bạch Kim I",
                "Kim Cương V", "Kim Cương IV", "Kim Cương III", "Kim Cương II", "Kim Cương I",
                "Cao Thủ", "Chiến Tướng", "Chiến Thần"
            };

            string currentRank = user.CurrentRank ?? "Đồng V";
            int currentStars = user.CurrentStars;

            int rankIndex = rankList.IndexOf(currentRank);
            if (rankIndex == -1) rankIndex = 0;

            if (model.IsWin)
            {
                currentStars++; // Thắng cộng 1 sao

                // Cập nhật logic: Tách riêng xử lý cho nhóm rank cao cấp tích lũy tối đa 50 sao
                if (currentRank == "Cao Thủ")
                {
                    if (currentStars > 50) // Đủ 50 sao nhảy lên Chiến Tướng
                    {
                        currentRank = "Chiến Tướng";
                        currentStars = 1;
                    }
                }
                else if (currentRank == "Chiến Tướng")
                {
                    if (currentStars > 50) // Đủ 50 sao nhảy lên Chiến Thần
                    {
                        currentRank = "Chiến Thần";
                        currentStars = 1;
                    }
                }
                else if (currentRank == "Chiến Thần")
                {
                    // Chiến Thần tích sao vô hạn để đua Top, không có trần giới hạn bậc nữa
                }
                else
                {
                    // Các rank bình thường từ Đồng đến Kim Cương (Cứ đạt qua mốc 5 sao là lên hạng)
                    if (currentStars > 5)
                    {
                        if (rankIndex < rankList.IndexOf("Kim Cương I"))
                        {
                            rankIndex++;
                            currentRank = rankList[rankIndex];
                            currentStars = 1;
                        }
                        else // Đang ở Kim Cương I mà vượt qua 5 sao thì lên Cao Thủ
                        {
                            currentRank = "Cao Thủ";
                            currentStars = 1;
                        }
                    }
                }
            }
            else
            {
                currentStars--; // Thua trừ 1 sao

                if (currentStars < 0)
                {
                    // Khấu trừ rớt hạng an toàn cho các bậc cao cấp
                    if (currentRank == "Chiến Thần")
                    {
                        currentRank = "Chiến Tướng";
                        currentStars = 50; // Trở về mốc trần 50 sao của Chiến Tướng
                    }
                    else if (currentRank == "Chiến Tướng")
                    {
                        currentRank = "Cao Thủ";
                        currentStars = 50; // Trở về mốc trần 50 sao của Cao Thủ
                    }
                    else if (currentRank == "Cao Thủ")
                    {
                        currentRank = "Kim Cương I";
                        currentStars = 5; // Xuống Kim Cương I và giữ nguyên kịch khung 5 sao để cày lại
                    }
                    else
                    {
                        // Logic tụt hạng dành cho các nhóm rank thường từ Đồng đến Kim Cương
                        if (rankIndex > 0)
                        {
                            rankIndex--;
                            currentRank = rankList[rankIndex];
                            currentStars = 5;
                        }
                        else
                        {
                            currentStars = 0; // Thấp nhất chạm đáy là Đồng V 0 sao
                        }
                    }
                }
            }

            // Lưu dữ liệu mới tính toán xong vào User
            user.CurrentRank = currentRank;
            user.CurrentStars = currentStars;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest("Lỗi khi cập nhật dữ liệu rank.");

            return Ok(new { newRank = currentRank, newStars = currentStars });
        }

        // 5. API LẤY DANH SÁCH BẢNG XẾP HẠNG NGƯỜI CHƠI THẬT
        [HttpGet]
        public async Task<IActionResult> GetLeaderboard()
        {
            var topPlayers = await _context.Users
                .OrderByDescending(u => u.CurrentStars)
                .Take(10)
                .Select(u => new {
                    fullName = u.FullName ?? u.UserName,
                    rankName = u.CurrentRank ?? "Đồng V",
                    stars = u.CurrentStars
                })
                .ToListAsync();

            return Json(topPlayers);
        }
    }

    // ====== CÁC CLASS MODEL PHỤ TRỢ (Đã nằm đúng phân vùng cấu trúc) ======
    public class AnswerCheckModel
    {
        public int QuestionId { get; set; }
        public string SelectedAnswer { get; set; }
    }

    public class MatchResultModel
    {
        public bool IsWin { get; set; }
    }
}