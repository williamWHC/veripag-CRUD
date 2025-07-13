using System;
using Orders.Domain.Enums;
using System.ComponentModel;
using System.Reflection;

namespace Orders.Application.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        
        public string StatusName => Status.ToString();
        
        public string StatusDescription => GetEnumDescription(Status);
        
        // Formatação do valor total
        public string FormattedTotalAmount => TotalAmount.ToString("C2");
        
        // Formatação da data
        public string FormattedOrderDate => OrderDate.ToString("dd/MM/yyyy HH:mm:ss");
        
        private static string GetEnumDescription(OrderStatus status)
        {
            var field = status.GetType().GetField(status.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? status.ToString();
        }
    }
} 