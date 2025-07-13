using System.ComponentModel.DataAnnotations;

namespace Orders.Application.DTOs
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome do cliente deve ter entre 2 e 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor total é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor total deve ser maior que zero")]
        public decimal TotalAmount { get; set; }
    }
} 