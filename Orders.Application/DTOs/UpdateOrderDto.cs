using System.ComponentModel.DataAnnotations;
using Orders.Domain.Enums;

namespace Orders.Application.DTOs
{
    public class UpdateOrderDto
    {
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome do cliente deve ter entre 2 e 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor total é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor total deve ser maior que zero")]
        public decimal TotalAmount { get; set; }
        
        [Required(ErrorMessage = "Status é obrigatório")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Status deve ser um valor válido")]
        public OrderStatus Status { get; set; }
    }
} 