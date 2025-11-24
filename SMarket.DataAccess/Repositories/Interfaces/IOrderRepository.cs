using SMarket.DataAccess.Models;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility.Enums;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetListOrdersAsync(int userId, RoleType userRole, ListOrderSearchCondition condition);
        Task<int> CountListOrdersAsync(int userId, RoleType userRole, ListOrderSearchCondition condition);
        Task<IEnumerable<Order>> GetBuyerOrdersAsync(int userId, ListOrderSearchCondition condition);
        Task<int> CountBuyerOrdersAsync(int userId, ListOrderSearchCondition condition);
        Task<IEnumerable<OrderStatus>> GetListOrderStatusesAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> CreateOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task UpdateOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task<Order> UpdateOrderStatusAsync(int id, int statusId);
        Task DeleteOrderAsync(int id);
    }
}
