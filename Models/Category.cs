using System.ComponentModel.DataAnnotations;

namespace WebGiaoDucGioiTinh.Models;

public class Category
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
