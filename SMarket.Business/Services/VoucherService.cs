using SMarket.Business.DTOs.Voucher;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.Utility.Enums;

namespace SMarket.Business.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly ICustomMapper _mapper;

        public VoucherService(IVoucherRepository voucherRepository, ICustomMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VoucherDto>> GetAllVouchersAsync()
        {
            var vouchers = await _voucherRepository.GetAllVouchersAsync();
            return _mapper.Map<Voucher, VoucherDto>(vouchers);
        }

        public async Task<VoucherDto?> GetVoucherByIdAsync(int id)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
            return voucher != null ? _mapper.Map<Voucher, VoucherDto>(voucher) : null;
        }

        public async Task<VoucherDto?> GetVoucherByCodeAsync(string code)
        {
            var voucher = await _voucherRepository.GetVoucherByCodeAsync(code);
            return voucher != null ? _mapper.Map<Voucher, VoucherDto>(voucher) : null;
        }

        public async Task<VoucherDto> CreateVoucherAsync(CreateVoucherDto createVoucherDto)
        {
            // Validate discount type
            if (createVoucherDto.DiscountType != "Percentage" && createVoucherDto.DiscountType != "Fixed")
            {
                throw new ArgumentException("Discount type must be either 'Percentage' or 'Fixed'");
            }

            // Validate percentage discount
            if (createVoucherDto.DiscountType == "Percentage" && createVoucherDto.DiscountAmount > 100)
            {
                throw new ArgumentException("Percentage discount cannot exceed 100%");
            }

            // Validate date range
            if (createVoucherDto.EndDate <= createVoucherDto.StartDate)
            {
                throw new ArgumentException("End date must be after start date");
            }

            // Check if code is unique
            if (!await _voucherRepository.IsCodeUniqueAsync(createVoucherDto.Code))
            {
                throw new ArgumentException("Voucher code already exists");
            }

            var voucher = _mapper.Map<CreateVoucherDto, Voucher>(createVoucherDto);
            var createdVoucher = await _voucherRepository.CreateVoucherAsync(voucher);

            return _mapper.Map<Voucher, VoucherDto>(createdVoucher);
        }

        public async Task<VoucherDto> UpdateVoucherAsync(int id, UpdateVoucherDto updateVoucherDto)
        {
            var existingVoucher = await _voucherRepository.GetVoucherByIdAsync(id);
            if (existingVoucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }

            // Validate discount type if provided
            if (!string.IsNullOrEmpty(updateVoucherDto.DiscountType) &&
                updateVoucherDto.DiscountType != "Percentage" &&
                updateVoucherDto.DiscountType != "Fixed")
            {
                throw new ArgumentException("Discount type must be either 'Percentage' or 'Fixed'");
            }

            // Validate percentage discount if provided
            if (updateVoucherDto.DiscountType == "Percentage" &&
                updateVoucherDto.DiscountAmount.HasValue &&
                updateVoucherDto.DiscountAmount > 100)
            {
                throw new ArgumentException("Percentage discount cannot exceed 100%");
            }

            // Validate date range if both dates are provided
            if (updateVoucherDto.StartDate.HasValue && updateVoucherDto.EndDate.HasValue)
            {
                if (updateVoucherDto.EndDate <= updateVoucherDto.StartDate)
                {
                    throw new ArgumentException("End date must be after start date");
                }
            }

            // Create a new entity with only the ID and map the update fields to it
            var voucherToUpdate = new Voucher { Id = id };
            _mapper.Map(updateVoucherDto, voucherToUpdate);

            var updatedVoucher = await _voucherRepository.UpdateVoucherAsync(voucherToUpdate);
            return _mapper.Map<Voucher, VoucherDto>(updatedVoucher);
        }

        public async Task DeleteVoucherAsync(int id)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(id);
            if (voucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }

            await _voucherRepository.DeleteVoucherAsync(id);
        }

        public async Task<IEnumerable<VoucherDto>> GetActiveVouchersAsync()
        {
            var vouchers = await _voucherRepository.GetActiveVouchersAsync();
            return _mapper.Map<Voucher, VoucherDto>(vouchers);
        }

        public async Task<IEnumerable<VoucherDto>> GetUserVouchersAsync(int userId)
        {
            var vouchers = await _voucherRepository.GetUserVouchersAsync(userId);
            return _mapper.Map<Voucher, VoucherDto>(vouchers);
        }

        public async Task<VoucherDto> AssignVoucherToUserAsync(int userId, int voucherId)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);
            if (voucher == null)
            {
                throw new ArgumentException("Voucher not found");
            }

            var existingAssignment = await _voucherRepository.GetUserVoucherAsync(userId, voucherId);
            if (existingAssignment != null)
            {
                throw new ArgumentException("Voucher is already assigned to this user");
            }

            var userVoucher = new UserVoucher
            {
                UserId = userId,
                VoucherId = voucherId
            };

            await _voucherRepository.AssignVoucherToUserAsync(userVoucher);
            return _mapper.Map<Voucher, VoucherDto>(voucher);
        }

        public async Task RemoveVoucherFromUserAsync(int userId, int voucherId)
        {
            var existingAssignment = await _voucherRepository.GetUserVoucherAsync(userId, voucherId);
            if (existingAssignment == null)
            {
                throw new ArgumentException("Voucher assignment not found");
            }

            await _voucherRepository.RemoveVoucherFromUserAsync(userId, voucherId);
        }

        public async Task<IEnumerable<VoucherStatusDto>> GetVoucherStatusesAsync()
        {
            var statuses = await _voucherRepository.GetVoucherStatusesAsync();
            return _mapper.Map<VoucherStatus, VoucherStatusDto>(statuses);
        }

        public async Task<VoucherApplicationResult> ValidateVoucherAsync(int voucherId)
        {
            var voucher = await _voucherRepository.GetVoucherByIdAsync(voucherId);

            if (voucher == null)
            {
                return new VoucherApplicationResult
                {
                    Message = "Voucher not found."
                };
            }

            var voucherDto = _mapper.Map<Voucher, VoucherDto>(voucher);

            if (voucher.StatusId != (int)VoucherStatuses.Active)
            {
                return new VoucherApplicationResult
                {
                    Message = "Voucher is not active.",
                    Voucher = voucherDto
                };
            }

            var now = DateTime.UtcNow;
            if (now < voucher.StartDate)
            {
                return new VoucherApplicationResult
                {
                    Message = "Voucher is not yet valid.",
                    Voucher = voucherDto
                };
            }

            if (now > voucher.EndDate)
            {
                return new VoucherApplicationResult
                {
                    Message = "Voucher has expired.",
                    Voucher = voucherDto
                };
            }

            if (voucher.UsageCount >= voucher.UsageLimit)
            {
                return new VoucherApplicationResult
                {
                    Message = "Voucher usage limit has been reached.",
                    Voucher = voucherDto
                };
            }

            return new VoucherApplicationResult
            {
                Message = "Voucher is valid and available.",
                Voucher = voucherDto
            };
        }

        public async Task ApplyVoucherAsync(int? voucherId)
        {
            if (voucherId.HasValue)
            {
                await _voucherRepository.IncrementUsageCountAsync(voucherId.Value);
            }
        }
    }
}
