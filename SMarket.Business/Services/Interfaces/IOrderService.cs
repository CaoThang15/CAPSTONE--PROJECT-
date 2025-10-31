using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Order;
using SMarket.Business.Enums;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility.Enums;

namespace SMarket.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginationResult<OrderDto>> GetListOrdersAsync(int userId, RoleType userRole, ListOrderSearchCondition condition);
        Task<PaginationResult<OrderDto>> GetBuyerOrdersAsync(int userId, ListOrderSearchCondition condition);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderStatusDto>> GetListOrderStatusesAsync();
        Task<OrderDto> CreateOrderAsync(CreateOrUpdateOrder order, int userId);
        Task UpdateOrderAsync(int id, CreateOrUpdateOrder order);
        Task<OrderDto> UpdateOrderStatusAsync(int id, int statusId);
        Task DeleteOrderAsync(int id);
    }
}
