using Orders.Domain.Enums;

namespace Orders.Application.Exceptions
{
    public class OrderCannotBeDeletedException : InvalidOperationException
    {
        public OrderCannotBeDeletedException(OrderStatus status) 
            : base($"Não é possível excluir um pedido com status '{status}'")
        {
            Status = status;
        }

        public OrderStatus Status { get; }
    }
} 