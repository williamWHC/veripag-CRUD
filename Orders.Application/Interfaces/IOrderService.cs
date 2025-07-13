using Orders.Application.DTOs;

namespace Orders.Application.Interfaces
{
    public interface IOrderService
    {
        Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync();
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id);
        Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<ApiResponse<OrderDto>> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);
        Task<ApiResponse<bool>> DeleteOrderAsync(int id);
    }
} 