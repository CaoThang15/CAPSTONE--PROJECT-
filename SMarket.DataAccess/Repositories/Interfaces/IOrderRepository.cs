using SMarket.DataAccess.Models;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetListOrdersAsync(ListOrderSearchCondition condition);
        Task<int> CountListOrdersAsync(ListOrderSearchCondition condition);
        Task<IEnumerable<OrderStatus>> GetListOrderStatusesAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task CreateOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task UpdateOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task UpdateOrderStatusAsync(int id, int statusId);
        Task DeleteOrderAsync(int id);
    }
}
