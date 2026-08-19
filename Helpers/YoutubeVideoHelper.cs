namespace WebGiaoDucGioiTinh.Helpers;

public static class YoutubeVideoHelper
{
    /// <summary>Trả về URL nhúng iframe hoặc null nếu không có / không nhận dạng được.</summary>
    public static string? GetEmbedUrl(string? videoUrl)
    {
        if (string.IsNullOrWhiteSpace(videoUrl))
            return null;

        var u = videoUrl.Trim();

        if (u.Contains("youtube.com/embed/", StringComparison.OrdinalIgnoreCase))
            return u.Split('?', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0];

        var watchIdx = u.IndexOf("watch?v=", StringComparison.OrdinalIgnoreCase);
        if (watchIdx >= 0)
        {
            var after = u[(watchIdx + 8)..];
            var id = after.Split(['&', '#'], StringSplitOptions.RemoveEmptyEntries)[0];
            if (id.Length > 0)
                return $"https://www.youtube.com/embed/{id}";
        }

        if (u.Contains("youtu.be/", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var uri = new Uri(u);
                var id = uri.AbsolutePath.Trim('/');
                if (id.Length > 0)
                    return $"https://www.youtube.com/embed/{id}";
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        return u.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? u : null;
    }
}
