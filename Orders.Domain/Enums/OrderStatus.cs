using System.ComponentModel;

namespace Orders.Domain.Enums
{
    public enum OrderStatus
    {
        [Description("Criado")]
        Created = 0,
        
        [Description("Pago")]
        Paid = 1,
        
        [Description("Enviado")]
        Shipped = 2,
        
        [Description("Cancelado")]
        Cancelled = 3
    }
} 