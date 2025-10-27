using Microsoft.EntityFrameworkCore;
using Npgsql;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.Utility.Enums;

namespace SMarket.DataAccess.Repositories
{
    public class VoucherRepository : IVoucherRepository
    {
        private readonly AppDbContext _context;

        public VoucherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Voucher>> GetAllVouchersAsync()
        {
            return await _context.Vouchers
                .Where(v => !v.IsDeleted)
                .Include(v => v.Status)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<Voucher?> GetVoucherByIdAsync(int id)
        {
            return await _context.Vouchers
                .Where(v => v.Id == id && !v.IsDeleted)
                .Include(v => v.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<Voucher?> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers
                .Where(v => v.Code == code && !v.IsDeleted)
                .Include(v => v.Status)
                .FirstOrDefaultAsync();
        }

        public async Task<Voucher> CreateVoucherAsync(Voucher voucher)
        {
            voucher.CreatedAt = DateTime.UtcNow;
            voucher.UsageCount = 0;

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();

            return await GetVoucherByIdAsync(voucher.Id) ?? voucher;
        }

        public async Task<Voucher> UpdateVoucherAsync(Voucher voucher)
        {
            var existingVoucher = await _context.Vouchers.FindAsync(voucher.Id);
            if (existingVoucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }

            // Use Entry to track only modified properties
            var entry = _context.Entry(existingVoucher);

            // Copy non-null values from the input voucher
            if (voucher.Description != null)
            {
                existingVoucher.Description = voucher.Description;
                entry.Property(e => e.Description).IsModified = true;
            }

            if (voucher.DiscountType != null)
            {
                existingVoucher.DiscountType = voucher.DiscountType;
                entry.Property(e => e.DiscountType).IsModified = true;
            }

            if (voucher.DiscountAmount != 0)
            {
                existingVoucher.DiscountAmount = voucher.DiscountAmount;
                entry.Property(e => e.DiscountAmount).IsModified = true;
            }

            if (voucher.StartDate != default(DateTime))
            {
                existingVoucher.StartDate = voucher.StartDate;
                entry.Property(e => e.StartDate).IsModified = true;
            }

            if (voucher.EndDate != default(DateTime))
            {
                existingVoucher.EndDate = voucher.EndDate;
                entry.Property(e => e.EndDate).IsModified = true;
            }

            if (voucher.UsageLimit != 0)
            {
                existingVoucher.UsageLimit = voucher.UsageLimit;
                entry.Property(e => e.UsageLimit).IsModified = true;
            }

            if (voucher.StatusId != 0)
            {
                existingVoucher.StatusId = voucher.StatusId;
                entry.Property(e => e.StatusId).IsModified = true;
            }

            existingVoucher.UpdatedAt = DateTime.UtcNow;
            entry.Property(e => e.UpdatedAt).IsModified = true;

            await _context.SaveChangesAsync();

            return await GetVoucherByIdAsync(existingVoucher.Id) ?? existingVoucher;
        }

        public async Task DeleteVoucherAsync(int id)
        {
            var voucher = await _context.Vouchers.FindAsync(id);
            if (voucher != null)
            {
                voucher.IsDeleted = true;
                voucher.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null)
        {
            var query = _context.Vouchers.Where(v => v.Code == code && !v.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(v => v.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Voucher>> GetActiveVouchersAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.Vouchers
                .Where(v => !v.IsDeleted &&
                           v.StatusId == (int)VoucherStatuses.Active &&
                           v.StartDate <= now &&
                           v.EndDate >= now &&
                           v.UsageCount < v.UsageLimit)
                .Include(v => v.Status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Voucher>> GetUserVouchersAsync(int userId)
        {
            return await _context.UserVouchers
                .Where(uv => uv.UserId == userId && !uv.IsDeleted)
                .Include(uv => uv.Voucher)
                .ThenInclude(v => v!.Status)
                .Select(uv => uv.Voucher!)
                .Where(v => !v.IsDeleted)
                .ToListAsync();
        }

        public async Task<UserVoucher?> GetUserVoucherAsync(int userId, int voucherId)
        {
            return await _context.UserVouchers
                .Where(uv => uv.UserId == userId && uv.VoucherId == voucherId && !uv.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<UserVoucher> AssignVoucherToUserAsync(UserVoucher userVoucher)
        {
            userVoucher.CreatedAt = DateTime.UtcNow;

            _context.UserVouchers.Add(userVoucher);
            await _context.SaveChangesAsync();

            return userVoucher;
        }

        public async Task RemoveVoucherFromUserAsync(int userId, int voucherId)
        {
            var userVoucher = await GetUserVoucherAsync(userId, voucherId);
            if (userVoucher != null)
            {
                userVoucher.IsDeleted = true;
                userVoucher.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VoucherStatus>> GetVoucherStatusesAsync()
        {
            return await _context.VoucherStatuses
                .Where(vs => !vs.IsDeleted)
                .OrderBy(vs => vs.Id)
                .ToListAsync();
        }

        public async Task IncrementUsageCountAsync(int voucherId)
        {
            var rowsAffected = await _context.Database.ExecuteSqlRawAsync(
                @"UPDATE ""Vouchers"" 
                  SET ""UsageCount"" = ""UsageCount"" + 1, ""UpdatedAt"" = @updateTime
                  WHERE ""Id"" = @voucherId 
                    AND ""UsageCount"" < ""UsageLimit"" 
                    AND ""IsDeleted"" = false",
                new NpgsqlParameter("@voucherId", voucherId),
                new NpgsqlParameter("@updateTime", DateTime.UtcNow)
            );

            if (rowsAffected == 0)
            {
                var voucher = await _context.Vouchers.FindAsync(voucherId);
                if (voucher == null)
                {
                    throw new ArgumentException("Voucher not found");
                }
                else if (voucher.UsageCount >= voucher.UsageLimit)
                {
                    throw new InvalidOperationException("Voucher usage limit has been reached");
                }
                else
                {
                    throw new InvalidOperationException("Failed to increment voucher usage count");
                }
            }
        }
    }
}
