using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using DAL.Models.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data
{
    public class Intelligence_Book_APIContext : DbContext
    {
        public Intelligence_Book_APIContext (DbContextOptions<Intelligence_Book_APIContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DeliveryAddress> DeliveryAddresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // 1. Cấu hình Index cho Book Title
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title, "idx_book_title"); // Đặt tên index trực tiếp ở đây

            // 2. Cấu hình Index cho User Email (Unique)
            modelBuilder.Entity<UserAccount>()
                .HasIndex(u => u.Email, "idx_user_email")
                .IsUnique();

            // Cấu hình Enum: Lưu dạng STRING trong database để dễ đọc
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<UserAccount>()
                .Property(u => u.Role)
                .HasConversion<string>();

            // Cấu hình tiền tệ Decimal (15,2)
            var decimalProps = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal));

            foreach (var prop in decimalProps)
            {
                prop.SetPrecision(15);
                prop.SetScale(2);
            }

            // Thiết lập xóa Order thì xóa OrderItems (Cascade)
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DeliveryAddress>()
                .HasOne(d => d.UserAccount)
                .WithMany(u => u.DeliveryAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình thêm cho bảng Review (Ràng buộc Rating)
            modelBuilder.Entity<Review>()
                .ToTable(t => t.HasCheckConstraint("CK_Review_Rating", "Rating BETWEEN 1 AND 5"));

            // Nếu bạn muốn cấu hình Cascade Delete (Xóa User thì xóa giỏ hàng)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.UserAccount)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // Cấu hình Quan hệ Nhiều-Nhiều giữa Book và Category
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategories", // Tên bảng trung gian trong SQL
                    j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId")
                );
        }
    }
}

