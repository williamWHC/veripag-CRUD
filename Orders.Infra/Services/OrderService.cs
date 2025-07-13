using Microsoft.Extensions.Logging;
using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Application.Exceptions;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Infra.Repositories;

namespace Orders.Infra.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Recuperando todos os pedidos");
                var orders = await _orderRepository.GetAllAsync();
                var orderDtos = orders.Select(MapToDto);
                
                _logger.LogInformation("Recuperados {Count} pedidos", orderDtos.Count());
                return ApiResponse<IEnumerable<OrderDto>>.SuccessResult(orderDtos, "Pedidos recuperados com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao recuperar pedidos");
                throw;
            }
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id)
        {
            try
            {
                ValidateOrderId(id);
                _logger.LogInformation("Buscando pedido com ID: {OrderId}", id);

                var order = await _orderRepository.GetByIdAsync(id);
                if (order == null)
                {
                    _logger.LogWarning("Pedido não encontrado com ID: {OrderId}", id);
                    throw new OrderNotFoundException(id);
                }

                _logger.LogInformation("Pedido encontrado com ID: {OrderId}", id);
                return ApiResponse<OrderDto>.SuccessResult(MapToDto(order), "Pedido encontrado com sucesso");
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedido com ID: {OrderId}", id);
                throw;
            }
        }

        public async Task<ApiResponse<OrderDto>> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                ValidateCreateOrderData(createOrderDto);
                _logger.LogInformation("Criando novo pedido para cliente: {CustomerName}", createOrderDto.CustomerName);

                var order = new Order(createOrderDto.CustomerName, createOrderDto.TotalAmount);
                ValidateOrder(order);

                var createdOrder = await _orderRepository.CreateAsync(order);
                
                _logger.LogInformation("Pedido criado com sucesso. ID: {OrderId}", createdOrder.Id);
                return ApiResponse<OrderDto>.SuccessResult(MapToDto(createdOrder), "Pedido criado com sucesso");
            }
            catch (InvalidOrderDataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido para cliente: {CustomerName}", createOrderDto.CustomerName);
                throw;
            }
        }

        public async Task<ApiResponse<OrderDto>> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            try
            {
                ValidateOrderId(id);
                ValidateUpdateOrderData(updateOrderDto);
                _logger.LogInformation("Atualizando pedido com ID: {OrderId}", id);

                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Pedido não encontrado para atualização. ID: {OrderId}", id);
                    throw new OrderNotFoundException(id);
                }

                ValidateStatusTransition(existingOrder.Status, updateOrderDto.Status);

                var updatedOrder = new Order
                {
                    Id = id,
                    CustomerName = updateOrderDto.CustomerName,
                    OrderDate = existingOrder.OrderDate,
                    TotalAmount = updateOrderDto.TotalAmount,
                    Status = updateOrderDto.Status
                };

                ValidateOrder(updatedOrder);

                var result = await _orderRepository.UpdateAsync(id, updatedOrder);
                if (result == null)
                {
                    throw new InvalidOperationException("Erro ao atualizar pedido");
                }

                _logger.LogInformation("Pedido atualizado com sucesso. ID: {OrderId}", id);
                return ApiResponse<OrderDto>.SuccessResult(MapToDto(result), "Pedido atualizado com sucesso");
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (InvalidOrderDataException)
            {
                throw;
            }
            catch (InvalidStatusTransitionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar pedido com ID: {OrderId}", id);
                throw;
            }
        }

        public async Task<ApiResponse<bool>> DeleteOrderAsync(int id)
        {
            try
            {
                ValidateOrderId(id);
                _logger.LogInformation("Excluindo pedido com ID: {OrderId}", id);

                var existingOrder = await _orderRepository.GetByIdAsync(id);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Pedido não encontrado para exclusão. ID: {OrderId}", id);
                    throw new OrderNotFoundException(id);
                }

                // Verifica se pode ser deletado (só pedidos criados)
                if (!existingOrder.CanBeDeleted())
                {
                    _logger.LogWarning("Pedido não pode ser excluído. Status: {Status}, ID: {OrderId}", existingOrder.Status, id);
                    throw new InvalidStatusTransitionException(existingOrder.Status, OrderStatus.Cancelled);
                }

                var result = await _orderRepository.DeleteAsync(id);
                if (!result)
                {
                    throw new InvalidOperationException("Erro ao excluir pedido");
                }

                _logger.LogInformation("Pedido excluído com sucesso. ID: {OrderId}", id);
                return ApiResponse<bool>.SuccessResult(true, "Pedido excluído com sucesso");
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (InvalidStatusTransitionException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir pedido com ID: {OrderId}", id);
                throw;
            }
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status
            };
        }

        private static void ValidateOrderId(int id)
        {
            if (id <= 0)
                throw new InvalidOrderDataException("ID do pedido é inválido", nameof(id));
        }

        private static void ValidateCreateOrderData(CreateOrderDto createOrderDto)
        {
            if (string.IsNullOrWhiteSpace(createOrderDto.CustomerName))
                throw new InvalidOrderDataException("Nome do cliente é obrigatório", nameof(createOrderDto.CustomerName));

            if (createOrderDto.TotalAmount <= 0)
                throw new InvalidOrderDataException("Valor total deve ser maior que zero", nameof(createOrderDto.TotalAmount));
        }

        private static void ValidateUpdateOrderData(UpdateOrderDto updateOrderDto)
        {
            if (string.IsNullOrWhiteSpace(updateOrderDto.CustomerName))
                throw new InvalidOrderDataException("Nome do cliente é obrigatório", nameof(updateOrderDto.CustomerName));

            if (updateOrderDto.TotalAmount <= 0)
                throw new InvalidOrderDataException("Valor total deve ser maior que zero", nameof(updateOrderDto.TotalAmount));
        }

        private static void ValidateOrder(Order order)
        {
            if (!order.IsValid())
                throw new InvalidOrderDataException("Dados do pedido são inválidos");
        }

        // Regras de transição de status do pedido
        private static void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            var isValid = currentStatus switch
            {
                OrderStatus.Created => newStatus == OrderStatus.Paid || newStatus == OrderStatus.Cancelled,
                OrderStatus.Paid => newStatus == OrderStatus.Shipped || newStatus == OrderStatus.Cancelled,
                OrderStatus.Shipped => false, // Pedido enviado não pode ser alterado
                OrderStatus.Cancelled => false, // Pedido cancelado não pode ser alterado
                _ => false
            };

            if (!isValid)
            {
                throw new InvalidStatusTransitionException(currentStatus, newStatus);
            }
        }
    }
} 