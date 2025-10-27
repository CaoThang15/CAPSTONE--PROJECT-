using SMarket.Business.DTOs.Common;
using SMarket.Business.DTOs.Order;
using SMarket.Business.Mappers;
using SMarket.Business.Services.Interfaces;
using SMarket.DataAccess.Models;
using SMarket.DataAccess.Repositories.Interfaces;
using SMarket.DataAccess.SearchCondition;

namespace SMarket.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomMapper _mapper;

        public OrderService(IOrderRepository orderRepository, ICustomMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<PaginationResult<OrderDto>> GetListOrdersAsync(ListOrderSearchCondition condition)
        {
            var ordersPaging = await _orderRepository.GetListOrdersAsync(condition);
            var orderDtos = _mapper.Map<Order, OrderDto>(ordersPaging);
            var total = await _orderRepository.CountListOrdersAsync(condition);

            return new PaginationResult<OrderDto>
            (
                currentPage: condition.Page,
                pageSize: condition.PageSize,
                totalItems: total,
                items: orderDtos
            );
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id)
                ?? throw new NotFoundException("Order not found");
            return _mapper.Map<Order, OrderDto>(order);
        }

        public async Task<IEnumerable<OrderStatusDto>> GetListOrderStatusesAsync()
        {
            var statuses = await _orderRepository.GetListOrderStatusesAsync();
            return _mapper.Map<OrderStatus, OrderStatusDto>(statuses);
        }

        public async Task CreateOrderAsync(CreateOrUpdateOrder createDto, int userId)
        {
            if (createDto.UserId == null || createDto.UserId == 0) createDto.UserId = userId;
            var order = _mapper.Map<CreateOrUpdateOrder, Order>(createDto);
            var orderDetails = _mapper.Map<CreateOrUpdateOrder, List<OrderDetail>>(createDto);
            await _orderRepository.CreateOrderAsync(order, orderDetails);
        }

        public async Task UpdateOrderAsync(int id, CreateOrUpdateOrder updateDto)
        {
            updateDto.Id = id;
            var order = _mapper.Map<CreateOrUpdateOrder, Order>(updateDto);
            var orderDetails = _mapper.Map<CreateOrUpdateOrder, List<OrderDetail>>(updateDto);
            await _orderRepository.UpdateOrderAsync(order, orderDetails);
        }

        public async Task UpdateOrderStatusAsync(int id, int statusId)
        {
            await _orderRepository.UpdateOrderStatusAsync(id, statusId);
        }

        public async Task DeleteOrderAsync(int id)
        {
            await _orderRepository.DeleteOrderAsync(id);
        }
    }
}
