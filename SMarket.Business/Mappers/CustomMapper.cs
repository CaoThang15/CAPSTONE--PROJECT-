using SMarket.Business.DTOs;
using SMarket.Business.DTOs.Cart;
using SMarket.Business.DTOs.Product;
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

            // CartItem mappings
            if (sourceType == typeof(CartItem) && destinationType == typeof(CartItemDto))
            {
                MapCartItemToDto((CartItem)(object)source!, (CartItemDto)(object)destination!);
                return true;
            }

            // Product mappings
            if (sourceType == typeof(Product) && destinationType == typeof(ProductItemDto))
            {
                MapProductToDto((Product)(object)source!, (ProductItemDto)(object)destination!);
                return true;
            }

            if (sourceType == typeof(CreateOrUpdateProductDto) && destinationType == typeof(Product))
            {
                MapCreateProductDtoToProduct((CreateOrUpdateProductDto)(object)source!, (Product)(object)destination!);
                return true;
            }

            if (sourceType == typeof(CreateOrUpdateProductDto) && destinationType == typeof(List<SharedFile>))
            {
                MapCreateProductDtoToSharedFiles((CreateOrUpdateProductDto)(object)source!, (List<SharedFile>)(object)destination!);
                return true;
            }

            if (sourceType == typeof(CreateOrUpdateProductDto) && destinationType == typeof(List<ProductProperty>))
            {
                MapCreateProductDtoToProperties((CreateOrUpdateProductDto)(object)source!, (List<ProductProperty>)(object)destination!);
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
            userDto.Ward = user.Ward;
            userDto.Province = user.Province;
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

            if (!string.IsNullOrEmpty(updateUserDto.Address))
                user.Address = updateUserDto.Address;

            if (!string.IsNullOrEmpty(updateUserDto.Ward))
                user.Ward = updateUserDto.Ward;

            if (!string.IsNullOrEmpty(updateUserDto.Province))
                user.Province = updateUserDto.Province;
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

        #region CartItem Mappings

        private void MapCartItemToDto(CartItem cartItem, CartItemDto cartItemDto)
        {
            cartItemDto.Id = cartItem.Id;
            cartItemDto.ProductId = cartItem.ProductId ?? 0;
            cartItemDto.Quantity = cartItem.Quantity;
            cartItemDto.UnitPrice = cartItem.UnitPrice;

            if (cartItem.Product != null)
            {
                cartItemDto.ProductName = cartItem.Product.Name ?? string.Empty;
                cartItemDto.ProductSlug = cartItem.Product.Slug ?? string.Empty;
                cartItemDto.ProductPrice = cartItem.Product.Price;
                cartItemDto.StockQuantity = cartItem.Product.StockQuantity;
                cartItemDto.ProductImage = cartItem.Product.SharedFiles?.FirstOrDefault()?.Path;
            }
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

        #region Product Mappings

        private static void MapProductToDto(Product product, ProductItemDto productDto)
        {
            productDto.Id = product.Id;
            productDto.CategoryId = product.CategoryId;
            productDto.Name = product.Name;
            productDto.Price = product.Price;
            productDto.Slug = product.Slug;
            productDto.Description = product.Description;
            productDto.StockQuantity = product.StockQuantity;
            productDto.Note = product.Note;
            productDto.IsNew = product.IsNew;
            productDto.IsAdminHide = product.IsAdminHide;
            productDto.IsHide = product.IsHide;
            productDto.SellerId = product.SellerId;
            productDto.SharedFiles = [];
            foreach (var file in product.SharedFiles)
            {
                if (file is not null && !string.IsNullOrEmpty(file.Path))
                    productDto.SharedFiles.Add(new SharedFileDto
                    {
                        Name = file.Name,
                        Path = file.Path
                    });
            }
            productDto.Properties = [];
            foreach (var property in product.ProductProperties)
            {
                productDto.Properties.Add(new ProductPropertyDto
                {
                    Id = property.Id,
                    Value = property.Value,
                    PropertyId = property.PropertyId,
                    PropertyName = property.Property?.Name,
                });
            }
        }

        private static void MapCreateProductDtoToProduct(CreateOrUpdateProductDto createDto, Product product)
        {
            product.Id = createDto.Id ?? 0;
            product.CategoryId = createDto.CategoryId;
            product.Name = createDto.Name;
            product.Price = createDto.Price;
            product.Slug = createDto.Slug;
            product.Description = createDto.Description;
            product.StockQuantity = createDto.StockQuantity;
            product.Note = createDto.Note;
            product.IsNew = createDto.IsNew;
            product.IsAdminHide = createDto.IsAdminHide;
            product.IsHide = createDto.IsHide;
            product.SellerId = createDto.SellerId;
        }

        private static void MapCreateProductDtoToSharedFiles(CreateOrUpdateProductDto createDto, List<SharedFile> sharedFiles)
        {
            sharedFiles.Clear();
            foreach (var file in createDto.SharedFiles)
            {
                sharedFiles.Add(new SharedFile
                {
                    Name = file.Name,
                    Path = file.Path,
                });
            }
        }
        
        private static void MapCreateProductDtoToProperties(CreateOrUpdateProductDto createDto, List<ProductProperty> properties)
        {
            properties.Clear();
            foreach(var property in createDto.Properties)
            {
                properties.Add(new ProductProperty
                {
                    Id = property.Id ?? 0,
                    Value = property.Value,
                    PropertyId = property.PropertyId,
                    ProductId = createDto.Id,
                });
            }
        }

        #endregion
    }
}
