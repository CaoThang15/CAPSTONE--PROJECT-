using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<UserVoucher> UserVouchers { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<VoucherStatus> VoucherStatuses { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<SystemNotification> SystemNotifications { get; set; }
        public DbSet<PersonalNotification> PersonalNotifications { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<ProductVector> ProductVectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<ProductVector>()
                .Property(v => v.Embedding)
                .HasColumnType("vector(768)");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.SellProducts)
                    .WithOne(p => p.Seller)
                    .HasForeignKey(p => p.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Orders)
                    .WithOne(o => o.User)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Status)
                    .WithMany()
                    .HasForeignKey(o => o.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(o => o.Voucher)
                    .WithMany(v => v.Orders)
                    .HasForeignKey(o => o.VoucherId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(c => c.Product)
                    .WithMany()
                    .HasForeignKey(c => c.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Voucher>(entity =>
            {
                entity.HasOne<VoucherStatus>(v => v.Status)
                    .WithMany(vs => vs.Vouchers)
                    .HasForeignKey(v => v.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
