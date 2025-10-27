using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Order;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginationResult<OrderDto>> GetListOrdersAsync(ListOrderSearchCondition condition);
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderStatusDto>> GetListOrderStatusesAsync();
        Task CreateOrderAsync(CreateOrUpdateOrder order, int userId);
        Task UpdateOrderAsync(int id, CreateOrUpdateOrder order);
        Task UpdateOrderStatusAsync(int id, int statusId);
        Task DeleteOrderAsync(int id);
    }
}
