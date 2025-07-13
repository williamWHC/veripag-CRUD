namespace Orders.Application.Exceptions
{
    public class OrderNotFoundException : Exception
    {
        public OrderNotFoundException(int id) 
            : base($"Pedido com ID {id} não foi encontrado")
        {
            OrderId = id;
        }

        public int OrderId { get; }
    }
} 