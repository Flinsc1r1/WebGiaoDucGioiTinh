using Microsoft.AspNetCore.Mvc;
using WebGiaoDucGioiTinh.Models;
using System.Text;
using System.Text.Json;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public ChatController(IConfiguration configuration)
    {
        _apiKey = configuration["Gemini:ApiKey"];
        _httpClient = new HttpClient();
    }

    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
    {
        if (string.IsNullOrEmpty(request.Prompt)) return BadRequest("Nhập gì đó đi bạn ơi!");

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={_apiKey}";

        // --- SỬA LỖI CÚ PHÁP VÀ TỐI ƯU PAYLOAD TẠI ĐÂY ---
        var payload = new
        {
            contents = new[] {
                new { parts = new[] { new { text = request.Prompt } } }
            },
            system_instruction = new
            {
                parts = new[] {
        new { text = @"Bạn là 'Trợ lý nhỏ' - một chuyên gia tâm lý và giáo dục giới tính thân thiện.
        - QUY TẮC QUAN TRỌNG: Không được trả lời theo kiểu 'nhắc lại từ khóa'. Bạn có kiến thức rộng lớn về mọi chủ đề giáo dục giới tính, sức khỏe sinh sản, tâm lý dậy thì.
        - CÁCH TRẢ LỜI: Khi bạn hỏi về bất cứ khái niệm nào (ví dụ: 'sinh sản là gì'), hãy giải thích trực tiếp, nhẹ nhàng và khoa học ngay lập tức.
        - PHONG CÁCH: Trò chuyện như một người bạn lớn tuổi đáng tin cậy, dùng ngôn ngữ ấm áp 😊." }
    }
            }
        }; // Thêm dấu đóng ngoặc và chấm phẩy ở đây

        var jsonPayload = JsonSerializer.Serialize(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(url, content);
            var resString = await response.Content.ReadAsStringAsync();

            // Trả kết quả về cho trình duyệt (Javascript sẽ nhận được chuỗi JSON này)
            return Ok(resString);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Lỗi server: {ex.Message}");
        }
    }
}