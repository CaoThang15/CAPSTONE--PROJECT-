using Microsoft.EntityFrameworkCore;
using SMarket.DataAccess.Common;
using SMarket.DataAccess.Context;
using SMarket.DataAccess.Enums;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;
using SMarket.Utility.Enums;

namespace SMarket.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetListOrdersAsync(int userId, RoleType userRole, ListOrderSearchCondition condition)
        {
            var query = _context.Orders
                .Where(o => !o.IsDeleted)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .AsQueryable();

            if (condition.PaymentMethodCode != null)
            {
                var method = ((PaymentMethod)condition.PaymentMethodCode).GetDescription();
                query = query.Where(o => o.PaymentMethod == method);
            }

            if (condition.StatusId != 0)
                query = query.Where(o => o.StatusId == condition.StatusId);


            if (!string.IsNullOrEmpty(condition.Keyword))
            {
                query = query.Where(o =>
                   o.User.Name.ToLower().Contains(condition.Keyword) ||
                   o.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(condition.Keyword)) ||
                   o.Id.ToString().Contains(condition.Keyword));
            }

            if (userRole == RoleType.Seller)
            {
                // Seller can only see orders from their store
                query = query.Where(o => o.OrderDetails.FirstOrDefault().Product.SellerId == userId);
            }

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((condition.Page - 1) * condition.PageSize)
                .Take(condition.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountListOrdersAsync(int userId, RoleType userRole, ListOrderSearchCondition condition)
        {
            var query = _context.Orders
                .Where(o => !o.IsDeleted)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Seller)
                .AsQueryable();

            if (condition.PaymentMethodCode != null)
            {
                var method = ((PaymentMethod)condition.PaymentMethodCode).GetDescription();
                query = query.Where(o => o.PaymentMethod == method);
            }

            if (condition.StatusId != 0)
                query = query.Where(o => o.StatusId == condition.StatusId);

            if (!string.IsNullOrEmpty(condition.Keyword))
            {
                query = query.Where(o =>
                   o.User.Name.ToLower().Contains(condition.Keyword) ||
                   o.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(condition.Keyword)) ||
                   o.Id.ToString().Contains(condition.Keyword));
            }

            if (userRole == RoleType.Seller)
            {
                // Seller can only see orders from their store
                query = query.Where(o => o.OrderDetails.FirstOrDefault().Product.SellerId == userId);
            }

            return await query.CountAsync();
        }

        public async Task<IEnumerable<Order>> GetBuyerOrdersAsync(int userId, ListOrderSearchCondition condition)
        {
            var query = _context.Orders
                .Where(o => !o.IsDeleted && o.UserId == userId)
                .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p!.Seller)
                .AsQueryable();

            if (condition.StatusId != 0)
                query = query.Where(o => o.StatusId == condition.StatusId);

            if (condition.PaymentMethodCode != null)
            {
                var method = ((PaymentMethod)condition.PaymentMethodCode).GetDescription();
                query = query.Where(o => o.PaymentMethod == method);
            }

            if (!string.IsNullOrEmpty(condition.Keyword))
            {
                var keyword = condition.Keyword.ToLower();

                query = query.Where(o =>
                    o.User.Name.ToLower().Contains(keyword) ||
                    o.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(keyword)) ||
                    o.Id.ToString().Contains(keyword));
            }
            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((condition.Page - 1) * condition.PageSize)
                .Take(condition.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountBuyerOrdersAsync(int userId, ListOrderSearchCondition condition)
        {
            var query = _context.Orders
               .Where(o => !o.IsDeleted && o.UserId == userId)
               .Include(o => o.Status)
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Seller)
                .AsQueryable();


            if (condition.StatusId != 0)
                query = query.Where(o => o.StatusId == condition.StatusId);

            if (condition.PaymentMethodCode != null)
            {
                var method = ((PaymentMethod)condition.PaymentMethodCode).GetDescription();
                query = query.Where(o => o.PaymentMethod == method);
            }

            if (!string.IsNullOrEmpty(condition.Keyword))
            {
                var keyword = condition.Keyword.ToLower();

                query = query.Where(o =>
                    o.User.Name.ToLower().Contains(keyword) ||
                    o.OrderDetails.Any(od => od.Product.Name.ToLower().Contains(keyword)) ||
                    o.Id.ToString().Contains(keyword));
            }

            return await query.CountAsync();
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
            return await _context.OrderStatuses.Where(d => !d.IsDeleted).OrderBy(c => c.Id).ToListAsync();
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                order.CreatedAt = DateTime.UtcNow;
                var newOrder = _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderId = order.Id;
                    orderDetail.CreatedAt = DateTime.UtcNow;
                    await _context.OrderDetails.AddAsync(orderDetail);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                await _context.Entry(newOrder.Entity)
                .Reference(o => o.User)
                .LoadAsync();
                return newOrder.Entity;
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

        public async Task<Order> UpdateOrderStatusAsync(int id, int statusId)
        {
            var order = await _context.Orders.FindAsync(id)
                ?? throw new InvalidOperationException("Not Found Order.");

            order.StatusId = statusId;
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            await _context.Entry(order)
                .Reference(o => o.Status)
                .LoadAsync();

            await _context.Entry(order)
                .Reference(o => o.User)
                .LoadAsync();

            return order;
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
