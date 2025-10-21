using SMarket.Business.DTOs;
using SMarket.Business.DTOs.Voucher;
using SMarket.DataAccess.Models;

namespace SMarket.Business.Mappers
{
    public class CustomMapper : ICustomMapper
    {
        public TDestination Map<TDestination>(object source) where TDestination : new()
        {
            if (source == null) return new TDestination();
            return Map<object, TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source) where TDestination : new()
        {
            if (source == null) return new TDestination();

            var destination = new TDestination();
            Map(source, destination);
            return destination;
        }

        public void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            if (source == null || destination == null) return;

            var sourceType = source.GetType();
            var destinationType = destination.GetType();

            // Handle specific mappings
            if (HandleSpecificMappings(source, destination, sourceType, destinationType))
                return;

            // Generic property mapping for same-named properties
            MapProperties(source, destination, sourceType, destinationType);
        }

        public IEnumerable<TDestination> Map<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new()
        {
            if (sources == null) return Enumerable.Empty<TDestination>();

            return sources.Select(source => Map<TSource, TDestination>(source));
        }

        public List<TDestination> MapToList<TSource, TDestination>(IEnumerable<TSource> sources) where TDestination : new()
        {
            return Map<TSource, TDestination>(sources).ToList();
        }

        #region Specific Entity Mappings

        private bool HandleSpecificMappings<TSource, TDestination>(TSource source, TDestination destination, Type sourceType, Type destinationType)
        {
            // User mappings
            if (sourceType == typeof(User) && destinationType == typeof(UserDto))
            {
                MapUserToDto((User)(object)source!, (UserDto)(object)destination!);
                return true;
            }

            if (sourceType == typeof(UpdateUserDto) && destinationType == typeof(User))
            {
                MapUpdateUserDtoToUser((UpdateUserDto)(object)source!, (User)(object)destination!);
                return true;
            }

            // Category mappings
            if (sourceType == typeof(Category) && destinationType == typeof(CategoryDto))
            {
                MapCategoryToDto((Category)(object)source!, (CategoryDto)(object)destination!);
                return true;
            }

            if (sourceType == typeof(CreateCategoryDto) && destinationType == typeof(Category))
            {
                MapCreateCategoryDtoToCategory((CreateCategoryDto)(object)source!, (Category)(object)destination!);
                return true;
            }

            if (sourceType == typeof(UpdateCategoryDto) && destinationType == typeof(Category))
            {
                MapUpdateCategoryDtoToCategory((UpdateCategoryDto)(object)source!, (Category)(object)destination!);
                return true;
            }

            // CategoryProperty mappings
            if (sourceType == typeof(CategoryProperty) && destinationType == typeof(CategoryPropertyDto))
            {
                MapCategoryPropertyToDto((CategoryProperty)(object)source!, (CategoryPropertyDto)(object)destination!);
                return true;
            }

            // Voucher mappings
            if (sourceType == typeof(Voucher) && destinationType == typeof(VoucherDto))
            {
                MapVoucherToDto((Voucher)(object)source!, (VoucherDto)(object)destination!);
                return true;
            }

            if (sourceType == typeof(CreateVoucherDto) && destinationType == typeof(Voucher))
            {
                MapCreateVoucherDtoToVoucher((CreateVoucherDto)(object)source!, (Voucher)(object)destination!);
                return true;
            }

            if (sourceType == typeof(UpdateVoucherDto) && destinationType == typeof(Voucher))
            {
                MapUpdateVoucherDtoToVoucher((UpdateVoucherDto)(object)source!, (Voucher)(object)destination!);
                return true;
            }

            // VoucherStatus mappings
            if (sourceType == typeof(VoucherStatus) && destinationType == typeof(VoucherStatusDto))
            {
                MapVoucherStatusToDto((VoucherStatus)(object)source!, (VoucherStatusDto)(object)destination!);
                return true;
            }

            return false;
        }

        #endregion

        #region User Mappings

        private void MapUserToDto(User user, UserDto userDto)
        {
            userDto.Id = user.Id;
            userDto.Name = user.Name;
            userDto.Email = user.Email;
            userDto.Phone = user.Phone;
            userDto.Avatar = user.Avatar;
            userDto.Address = user.Address;
            userDto.RoleId = user.RoleId;
        }

        private void MapUpdateUserDtoToUser(UpdateUserDto updateUserDto, User user)
        {
            if (!string.IsNullOrEmpty(updateUserDto.Name))
                user.Name = updateUserDto.Name;

            if (!string.IsNullOrEmpty(updateUserDto.Phone))
                user.Phone = updateUserDto.Phone;

            if (!string.IsNullOrEmpty(updateUserDto.Avatar))
                user.Avatar = updateUserDto.Avatar;

            // Handle address combination
            if (!string.IsNullOrEmpty(updateUserDto.Address) ||
                !string.IsNullOrEmpty(updateUserDto.Ward) ||
                !string.IsNullOrEmpty(updateUserDto.Province))
            {
                user.Address = $"{updateUserDto.Address}, {updateUserDto.Ward}, {updateUserDto.Province}".Trim(',', ' ');
            }
        }

        #endregion

        #region Category Mappings

        private void MapCategoryToDto(Category category, CategoryDto categoryDto)
        {
            categoryDto.Id = category.Id;
            categoryDto.Name = category.Name;
            categoryDto.Slug = category.Slug;
        }

        private void MapCreateCategoryDtoToCategory(CreateCategoryDto createDto, Category category)
        {
            category.Name = createDto.Name;
            category.Slug = createDto.Slug;
        }

        private void MapUpdateCategoryDtoToCategory(UpdateCategoryDto updateDto, Category category)
        {
            if (!string.IsNullOrEmpty(updateDto.Name))
                category.Name = updateDto.Name;

            if (!string.IsNullOrEmpty(updateDto.Slug))
                category.Slug = updateDto.Slug;
        }

        #endregion

        #region CategoryProperty Mappings

        private void MapCategoryPropertyToDto(CategoryProperty property, CategoryPropertyDto propertyDto)
        {
            propertyDto.Id = property.Id;
            propertyDto.Name = property.Name;
            propertyDto.CategoryId = property.CategoryId;
        }

        #endregion

        #region Voucher Mappings

        private void MapVoucherToDto(Voucher voucher, VoucherDto voucherDto)
        {
            voucherDto.Id = voucher.Id;
            voucherDto.Code = voucher.Code ?? string.Empty;
            voucherDto.Description = voucher.Description;
            voucherDto.DiscountType = voucher.DiscountType ?? string.Empty;
            voucherDto.DiscountAmount = voucher.DiscountAmount;
            voucherDto.StartDate = voucher.StartDate;
            voucherDto.EndDate = voucher.EndDate;
            voucherDto.UsageLimit = voucher.UsageLimit;
            voucherDto.UsageCount = voucher.UsageCount;
            voucherDto.StatusId = voucher.StatusId;
            voucherDto.StatusName = voucher.Status?.Name ?? string.Empty;
            voucherDto.CreatedAt = voucher.CreatedAt;
            voucherDto.UpdatedAt = voucher.UpdatedAt;
        }

        private void MapCreateVoucherDtoToVoucher(CreateVoucherDto createDto, Voucher voucher)
        {
            voucher.Code = createDto.Code;
            voucher.Description = createDto.Description;
            voucher.DiscountType = createDto.DiscountType;
            voucher.DiscountAmount = createDto.DiscountAmount;
            voucher.StartDate = createDto.StartDate;
            voucher.EndDate = createDto.EndDate;
            voucher.UsageLimit = createDto.UsageLimit;
            voucher.StatusId = createDto.StatusId;
        }

        private void MapUpdateVoucherDtoToVoucher(UpdateVoucherDto updateDto, Voucher voucher)
        {
            if (!string.IsNullOrEmpty(updateDto.Description))
                voucher.Description = updateDto.Description;

            if (!string.IsNullOrEmpty(updateDto.DiscountType))
                voucher.DiscountType = updateDto.DiscountType;

            if (updateDto.DiscountAmount.HasValue)
                voucher.DiscountAmount = updateDto.DiscountAmount.Value;

            if (updateDto.StartDate.HasValue)
                voucher.StartDate = updateDto.StartDate.Value;

            if (updateDto.EndDate.HasValue)
                voucher.EndDate = updateDto.EndDate.Value;

            if (updateDto.UsageLimit.HasValue)
                voucher.UsageLimit = updateDto.UsageLimit.Value;

            if (updateDto.StatusId.HasValue)
                voucher.StatusId = updateDto.StatusId.Value;
        }

        #endregion

        #region VoucherStatus Mappings

        private void MapVoucherStatusToDto(VoucherStatus status, VoucherStatusDto statusDto)
        {
            statusDto.Id = status.Id;
            statusDto.Name = status.Name ?? string.Empty;
        }

        #endregion

        #region Generic Property Mapping

        private void MapProperties<TSource, TDestination>(TSource source, TDestination destination, Type sourceType, Type destinationType)
        {
            var sourceProperties = sourceType.GetProperties().Where(p => p.CanRead);
            var destinationProperties = destinationType.GetProperties().Where(p => p.CanWrite).ToDictionary(p => p.Name, p => p);

            foreach (var sourceProperty in sourceProperties)
            {
                if (destinationProperties.TryGetValue(sourceProperty.Name, out var destinationProperty))
                {
                    if (destinationProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                    {
                        var value = sourceProperty.GetValue(source);
                        if (value != null)
                        {
                            destinationProperty.SetValue(destination, value);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
