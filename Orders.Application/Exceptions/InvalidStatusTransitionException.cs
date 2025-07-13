using Orders.Domain.Enums;

namespace Orders.Application.Exceptions
{
    public class InvalidStatusTransitionException : InvalidOperationException
    {
        public InvalidStatusTransitionException(OrderStatus currentStatus, OrderStatus newStatus) 
            : base($"Transição de status inválida: {currentStatus} -> {newStatus}")
        {
            CurrentStatus = currentStatus;
            NewStatus = newStatus;
        }

        public OrderStatus CurrentStatus { get; }
        public OrderStatus NewStatus { get; }
    }
} 