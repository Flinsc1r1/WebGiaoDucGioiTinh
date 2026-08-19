using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Data;

public static class SeedData
{
    /// <summary>
    /// Chèn dữ liệu mẫu nếu chưa có danh mục.
    /// Để seed lại cấu trúc mới: xóa dữ liệu các bảng Quizzes, Lessons, Categories (hoặc drop database) rồi chạy lại ứng dụng.
    /// </summary>
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var catPuberty = new Category { Name = "Tuổi dậy thì" };
        var catSafety = new Category { Name = "An toàn cá nhân" };
        var catReproductive = new Category { Name = "Sức khỏe sinh sản" };

        context.Categories.AddRange(catPuberty, catSafety, catReproductive);
        await context.SaveChangesAsync();

        // Video mẫu (YouTube embed) — có thể thay bằng nội dung giáo dục chính thức khi có.
        const string demoEmbed = "https://www.youtube.com/embed/M7lc1UVf-VE";

        var lessons = new List<Lesson>
        {
            new()
            {
                CategoryId = catPuberty.Id,
                Title = "Những thay đổi cơ thể trong tuổi dậy thì",
                Content = """
                    <p>Tuổi dậy thì là giai đoạn cơ thể phát triển về <strong>sinh lý</strong> và <strong>cảm xúc</strong>. Mỗi người bước vào giai đoạn này ở <em>nhịp riêng</em> — điều đó là bình thường.</p>
                    <p>Bạn có thể nhận thấy thay đổi về giọng nói, da, tăng trưởng chiều cao, hoặc những biến đổi khác. Thông tin đúng và sự đồng cảm giúp bạn hiểu mình hơn, giảm lo lắng vô cớ.</p>
                    <p>Hãy duy trì ngủ đủ, ăn uống đa dạng, vận động vừa sức và tìm nguồn tư vấn đáng tin khi cần.</p>
                    """,
                VideoUrl = demoEmbed,
            },
            new()
            {
                CategoryId = catPuberty.Id,
                Title = "Cảm xúc, tự trọng và hình ảnh cơ thể",
                Content = """
                    <p>Trong tuổi dậy thì, tâm trạng có thể thay đổi nhanh. Điều đó không có nghĩa là bạn “quá nhạy cảm” — não bộ và hormone đang điều chỉnh.</p>
                    <p><strong>Tự trọng</strong> là coi trọng bản thân: bạn xứng đáng được an toàn, được lắng nghe và được tôn trọng ranh giới.</p>
                    <p>Nếu cảm thấy buồn kéo dài hoặc tổn thương bản thân, hãy tìm người lớn hoặc chuyên gia sức khỏe tâm thần để được hỗ trợ.</p>
                    """,
                VideoUrl = demoEmbed,
            },
            new()
            {
                CategoryId = catSafety.Id,
                Title = "Ranh giới cá nhân và quyền nói \"không\"",
                Content = """
                    <p><strong>Ranh giới</strong> là giới hạn về thể chất và cảm xúc mà bạn có quyền thiết lập. Bạn có thể nói “không” khi không muốn ôm, chụp ảnh, hoặc chia sẻ thông tin riêng tư.</p>
                    <p>Người khác tôn trọng ranh giới của bạn là dấu hiệu của mối quan hệ lành mạnh. Không ai có quyền ép bạn vượt qua giới hạn đó.</p>
                    <p>Khi không chắc, hãy nhờ người lớn tin cậy đồng hành và ghi nhớ: an toàn quan trọng hơn “làm vui lòng người khác”.</p>
                    """,
                VideoUrl = demoEmbed,
            },
            new()
            {
                CategoryId = catSafety.Id,
                Title = "An toàn trên mạng và giao tiếp trực tuyến",
                Content = """
                    <p>Internet mang lại học tập và kết nối, nhưng cũng có rủi ro: lừa đảo, quấy rối, hoặc yêu cầu nội dung nhạy cảm.</p>
                    <p>Hãy bảo vệ thông tin cá nhân, cân nhắc trước khi gửi ảnh/tin nhắn, và sử dụng công cụ <strong>chặn — báo cáo</strong> khi cảm thấy không an toàn.</p>
                    <p>Nếu điều gì khiến bạn lo lắng, hãy lưu bằng chứng và báo cho người lớn hoặc cơ quan chức năng phù hợp.</p>
                    """,
                VideoUrl = demoEmbed,
            },
            new()
            {
                CategoryId = catReproductive.Id,
                Title = "Hiểu về kinh nguyệt và chăm sóc cơ bản",
                Content = """
                    <p>Kinh nguyệt là một phần sinh lý bình thường ở nhiều người có tử cung. Chu kỳ có thể không đều trong những năm đầu — điều đó thường gặp.</p>
                    <p>Học cách theo dõi chu kỳ, vệ sinh an toàn và nhận biết dấu hiệu cần khám (đau dữ dội, ra máu bất thường, v.v.) giúp bạn chủ động chăm sóc sức khỏe.</p>
                    <p>Khi có thắc mắc y khoa, nên hỏi phụ huynh/người lớn tin cậy hoặc cơ sở y tế.</p>
                    """,
                VideoUrl = demoEmbed,
            },
            new()
            {
                CategoryId = catReproductive.Id,
                Title = "Sức khỏe sinh sản: thông tin đúng, tôn trọng và an toàn",
                Content = """
                    <p>Sức khỏe sinh sản gắn với hiểu biết về cơ thể, phòng bệnh, và các lựa chọn có trách nhiệm khi trưởng thành.</p>
                    <p>Thông tin nên đến từ <strong>nguồn khoa học</strong> (y tế, giáo dục chính thống), không phải tin đồn trên mạng.</p>
                    <p>Biện pháp tránh thai và phòng STIs là chủ đề dành cho trao đổi với người có chuyên môn; mục tiêu của bài học là khuyến khích bạn đặt câu hỏi đúng chỗ và được tư vấn phù hợp lứa tuổi.</p>
                    """,
                VideoUrl = demoEmbed,
            },
        };

        context.Lessons.AddRange(lessons);
        await context.SaveChangesAsync();

        var quizzes = BuildQuizzesForLessons(lessons);
        context.Quizzes.AddRange(quizzes);
        await context.SaveChangesAsync();
    }

    private static List<Quiz> BuildQuizzesForLessons(List<Lesson> lessons)
    {
        var byTitle = lessons.ToDictionary(l => l.Title, l => l.Id);

        return new List<Quiz>
        {
            Q(byTitle["Những thay đổi cơ thể trong tuổi dậy thì"], "Điều nào phù hợp với tinh thần giáo dục giới tính nhân văn?",
                "So sánh cơ thể để xem ai “đúng chuẩn”.", "Coi sự khác biệt tiến độ dậy thì là bình thường và tôn trọng bản thân.", "Giấu kín mọi thắc mắc.", "Tin hoàn toàn vào lời đồn mạng.", "B"),
            Q(byTitle["Những thay đổi cơ thể trong tuổi dậy thì"], "Giấc ngủ và vận động vừa sức trong tuổi dậy thì giúp gì?",
                "Không quan trọng.", "Hỗ trợ phát triển thể chất, tâm lý và khả năng tập trung.", "Chỉ cần khi mệt.", "Thay thế dinh dưỡng.", "B"),
            Q(byTitle["Những thay đổi cơ thể trong tuổi dậy thì"], "Khi có thắc mắc sức khỏe, hướng tiếp cận nào thường phù hợp?",
                "Trao đổi với người lớn tin cậy hoặc cơ sở y tế khi cần.", "Tự chẩn đoán qua mạng.", "Bỏ qua hy vọng tự hết.", "Chia sẻ chi tiết riêng tư công khai.", "A"),

            Q(byTitle["Cảm xúc, tự trọng và hình ảnh cơ thể"], "Tự trọng (self-respect) có nghĩa gần nhất với điều nào?",
                "Chấp nhận bị đối xử tệ để giữ bạn bè.", "Coi trọng quyền được an toàn và được tôn trọng.", "Luôn đồng ý mọi yêu cầu.", "So sánh mình với người khác để tự ti.", "B"),
            Q(byTitle["Cảm xúc, tự trọng và hình ảnh cơ thể"], "Nếu cảm xúc tiêu cực kéo dài hoặc có ý nghĩ tự hại, bạn nên làm gì?",
                "Giữ kín một mình.", "Tìm người lớn tin cậy hoặc chuyên gia sức khỏe tâm thần.", "Lên mạng tìm “cách chữa” ngẫu nhiên.", "Trách bản thân là yếu đuối.", "B"),
            Q(byTitle["Cảm xúc, tự trọng và hình ảnh cơ thể"], "Hình ảnh cơ thể lành mạnh thường gắn với điều nào?",
                "Ước mình giống hệt người khác.", "Nhận ra cơ thể thay đổi theo thời gian và cần được chăm sóc, không bị sỉ nhục.", "Chỉ đẹp khi đạt một số đo cụ thể.", "Phải che giấu mọi khác biệt.", "B"),

            Q(byTitle["Ranh giới cá nhân và quyền nói \"không\""], "Ranh giới cá nhân là gì?",
                "Chỉ dành cho người lớn.", "Giới hạn thể chất/cảm xúc mà bạn có quyền đặt ra.", "Cách tránh nói chuyện với ai.", "Luôn cố định suốt đời.", "B"),
            Q(byTitle["Ranh giới cá nhân và quyền nói \"không\""], "Khi ai đó vượt ranh giới của bạn, điều nào phù hợp?",
                "Im lặng để khỏi căng thẳng.", "Rời khỏi tình huống và tìm hỗ trợ nếu cần.", "Tự trách mình.", "Đồng ý để tránh mất lòng.", "B"),
            Q(byTitle["Ranh giới cá nhân và quyền nói \"không\""], "“Không” có cần giải thích dài dòng mới hợp lệ không?",
                "Có, luôn phải giải thích chi tiết.", "Không — ranh giới của bạn có giá trị dù bạn nói ngắn gọn.", "Chỉ hợp lệ với người lạ.", "Chỉ khi có người chứng kiến.", "B"),

            Q(byTitle["An toàn trên mạng và giao tiếp trực tuyến"], "Khi bị yêu cầu gửi ảnh riêng tư từ người lạ, bạn nên ưu tiên điều gì?",
                "Gửi để chứng minh thiện chí.", "Từ chối, chặn/báo cáo và báo người lớn nếu lo lắng.", "Đăng công khai để “dằn mặt”.", "Trao đổi thêm để thuyết phục.", "B"),
            Q(byTitle["An toàn trên mạng và giao tiếp trực tuyến"], "Dấu hiệu nào gợi ý tài khoản có thể lừa đảo hoặc không an toàn?",
                "Luôn tôn trọng ranh giới của bạn.", "Ép bạn giữ bí mật với người lớn hoặc gấp rút yêu cầu thông tin/thanh toán.", "Không hỏi thông tin cá nhân.", "Cho bạn thời gian suy nghĩ.", "B"),
            Q(byTitle["An toàn trên mạng và giao tiếp trực tuyến"], "Khi bị quấy rối trực tuyến, việc nào hợp lý?",
                "Xóa bằng chứng ngay.", "Lưu bằng chứng (ảnh chụp), chặn/báo cáo và nhờ người lớn hỗ trợ.", "Trả đũa bằng lời lẽ tương tự.", "Tự chịu để khỏi phiền phức.", "B"),

            Q(byTitle["Hiểu về kinh nguyệt và chăm sóc cơ bản"], "Chu kỳ kinh nguyệt ở tuổi vị thành niên thường như thế nào?",
                "Luôn đều đặn tuyệt đối ngay từ đầu.", "Có thể chưa đều trong những năm đầu — thường là bình thường.", "Luôn là bệnh nếu không đều.", "Không cần theo dõi.", "B"),
            Q(byTitle["Hiểu về kinh nguyệt và chăm sóc cơ bản"], "Khi nào nên hỏi ý kiến chuyên gia y tế về kinh nguyệt?",
                "Chỉ khi đã trưởng thành.", "Khi đau dữ dội, ra máu bất thường, hoặc lo lắng kéo dài ảnh hưởng sinh hoạt.", "Không bao giờ cần.", "Chỉ khi có người khác bảo.", "B"),
            Q(byTitle["Hiểu về kinh nguyệt và chăm sóc cơ bản"], "Vệ sinh trong ngày hành kinh nên hướng tới điều gì?",
                "Dùng sản phẩm sạch, thay đúng hướng dẫn và rửa tay.", "Dùng chung đồ dùng với người khác.", "Tránh tắm rửa hoàn toàn.", "Bỏ qua nếu “ít máu”.", "A"),

            Q(byTitle["Sức khỏe sinh sản: thông tin đúng, tôn trọng và an toàn"], "Nguồn thông tin về sức khỏe sinh sản đáng tin thường là gì?",
                "Chỉ bình luận mạng xã hội.", "Cơ sở y tế, tổ chức giáo dục uy tín, hoặc chuyên gia có chứng chỉ.", "Tin nhắn ẩn danh.", "Quảng cáo không kiểm chứng.", "B"),
            Q(byTitle["Sức khỏe sinh sản: thông tin đúng, tôn trọng và an toàn"], "Thái độ nào phù hợp khi trao đổi về sức khỏe sinh sản ở lứa tuổi học sinh?",
                "Phán xét và đùa cợt.", "Tôn trọng, khoa học, và khuyến khích hỏi người có chuyên môn.", "Tránh hoàn toàn mọi chủ đề.", "Lan truyền tin đồn.", "B"),
            Q(byTitle["Sức khỏe sinh sản: thông tin đúng, tôn trọng và an toàn"], "Mục tiêu của giáo dục sức khỏe sinh sản trong nhà trường thường bao gồm gì?",
                "Khuyến khích hành vi mạo hiểm.", "Giúp học sinh hiểu cơ thể, phòng bệnh, và biết tìm hỗ trợ đúng chỗ.", "Thay thế vai trò của phụ huynh hoàn toàn.", "Chỉ dành cho một giới.", "B"),
        };
    }

    private static Quiz Q(int lessonId, string question, string a, string b, string c, string d, string correct) => new()
    {
        LessonId = lessonId,
        Question = question,
        OptionA = a,
        OptionB = b,
        OptionC = c,
        OptionD = d,
        CorrectAnswer = correct,
    };
}
