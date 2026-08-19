using System;
using System.ComponentModel.DataAnnotations;

namespace WebGiaoDucGioiTinh.Models
{
    public class Leaderboard
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        public int TotalScore { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.Now;
    }
}