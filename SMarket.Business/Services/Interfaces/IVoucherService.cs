using SMarket.Business.DTOs.Voucher;

namespace SMarket.Business.Services.Interfaces
{
    public interface IVoucherService
    {
        Task<IEnumerable<VoucherDto>> GetAllVouchersAsync();
        Task<VoucherDto?> GetVoucherByIdAsync(int id);
        Task<VoucherDto?> GetVoucherByCodeAsync(string code);
        Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto createVoucherDto);
        Task<VoucherDto> UpdateVoucherAsync(int id, UpdateVoucherDto updateVoucherDto);
        Task DeleteVoucherAsync(int id);
        Task<IEnumerable<VoucherDto>> GetActiveVouchersAsync();
        Task<IEnumerable<VoucherDto>> GetUserVouchersAsync(int userId);
        Task AssignVoucherToUserAsync(int userId, int voucherId);
        Task RemoveVoucherFromUserAsync(int userId, int voucherId);
        Task<IEnumerable<VoucherStatusDto>> GetVoucherStatusesAsync();
        Task<VoucherApplicationResult> ValidateVoucherAsync(int voucherId);
        Task ApplyVoucherAsync(int? voucherId);
    }
}
