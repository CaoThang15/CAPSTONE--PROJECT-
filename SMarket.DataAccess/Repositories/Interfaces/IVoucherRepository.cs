using SMarket.DataAccess.Models;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IVoucherRepository
    {
        Task<IEnumerable<Voucher>> GetAllVouchersAsync();
        Task<Voucher?> GetVoucherByIdAsync(int id);
        Task<Voucher?> GetVoucherByCodeAsync(string code);
        Task<Voucher> CreateVoucherAsync(Voucher voucher);
        Task<Voucher> UpdateVoucherAsync(Voucher voucher);
        Task DeleteVoucherAsync(int id);
        Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null);
        Task<IEnumerable<Voucher>> GetActiveVouchersAsync();
        Task<IEnumerable<Voucher>> GetUserVouchersAsync(int userId);
        Task<UserVoucher?> GetUserVoucherAsync(int userId, int voucherId);
        Task<UserVoucher> AssignVoucherToUserAsync(UserVoucher userVoucher);
        Task RemoveVoucherFromUserAsync(int userId, int voucherId);
        Task<IEnumerable<VoucherStatus>> GetVoucherStatusesAsync();
        Task IncrementUsageCountAsync(int voucherId);
    }
}
