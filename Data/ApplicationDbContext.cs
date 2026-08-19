using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebGiaoDucGioiTinh.Models;

namespace WebGiaoDucGioiTinh.Data
{
    // Đổi từ DbContext sang IdentityDbContext để hỗ trợ các bảng User, Role, Claim...
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Lesson> Lessons => Set<Lesson>();

        // Bảng gốc quản lý câu hỏi của bạn
        public DbSet<Quiz> Quizzes => Set<Quiz>();

        // Thêm dòng này làm "Cầu nối" (Alias) để QuizzesController.cs gọi _context.Questions không còn bị báo lỗi đỏ nữa
        public DbSet<Quiz> Questions => Quizzes;

        public DbSet<AnonymousQuestion> AnonymousQuestions => Set<AnonymousQuestion>();
        public DbSet<UserQuizProgress> UserQuizProgresses => Set<UserQuizProgress>();

        // Sửa lại chữ đằng sau thành FeaturedNewsList để hết bị trùng tên (Ambiguity)
        public DbSet<FeaturedNews> FeaturedNewsList { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Quan trọng: Phải gọi base.OnModelCreating để khởi tạo các bảng Identity
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<Lesson>(entity =>
            {
                entity.HasOne(l => l.Category)
                    .WithMany(c => c.Lessons)
                    .HasForeignKey(l => l.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasOne(q => q.Lesson)
                    .WithMany(l => l.Quizzes)
                    .HasForeignKey(q => q.LessonId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AnonymousQuestion>(entity =>
            {
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }
}