using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Enums;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetListOrdersAsync(ListOrderSearchCondition condition)
        {
            return await _context.Orders
                .Where(d => !d.IsDeleted)
                .Where(d => condition.PaymentMethodCode == null
                    || ((PaymentMethod)condition.PaymentMethodCode).GetDescription() == d.PaymentMethod)
                .Where(d => condition.StatusId == 0 || condition.StatusId == d.StatusId)
                .Where(d => condition.UserId == 0 || condition.UserId == d.UserId)
                .Include(d => d.Status)
                .Skip((condition.Page - 1) * condition.PageSize)
                .Take(condition.PageSize).ToListAsync();
        }

        public async Task<int> CountListOrdersAsync(ListOrderSearchCondition condition)
        {
            return await _context.Orders.Where(d => !d.IsDeleted)
                .Where(d => condition.PaymentMethodCode == null
                    || ((PaymentMethod)condition.PaymentMethodCode).GetDescription() == d.PaymentMethod)
                .Where(d => condition.StatusId == 0 || condition.StatusId == d.StatusId)
                .Where(d => condition.UserId == 0 || condition.UserId == d.UserId)
                .CountAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders.Where(d => !d.IsDeleted && d.Id == id)
                .Include(d => d.Status)
                .Include(d => d.Voucher)
                .Include(d => d.User)
                .Include(d => d.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p!.Seller)
                .Include(d => d.OrderDetails)
                    .ThenInclude(od => od.Product)
                        .ThenInclude(p => p!.SharedFiles)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<OrderStatus>> GetListOrderStatusesAsync()
        {
            return await _context.OrderStatuses.Where(d => !d.IsDeleted).ToListAsync();
        }

        public async Task CreateOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                order.CreatedAt = DateTime.UtcNow;
                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderId = order.Id;
                    orderDetail.CreatedAt = DateTime.UtcNow;
                    await _context.OrderDetails.AddAsync(orderDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                order.UpdatedAt = DateTime.UtcNow;
                _context.Orders.Update(order);

                var oldOrderDetail = await _context.OrderDetails
                    .Where(d => d.OrderId == order.Id).ToListAsync();
                _context.OrderDetails.RemoveRange(oldOrderDetail);

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderId = order.Id;
                    orderDetail.CreatedAt = DateTime.UtcNow;
                    await _context.OrderDetails.AddAsync(orderDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateOrderStatusAsync(int id, int statusId)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new InvalidOperationException("Not Found Order."); ;
            if (order != null)
            {
                order.StatusId = statusId;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new InvalidOperationException("Not Found Order."); ;
            if (order != null)
            {
                order.IsDeleted = true;
                order.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
