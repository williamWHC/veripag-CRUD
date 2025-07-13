using System;
using Orders.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Orders.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome do cliente deve ter no máximo 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor total deve ser maior que zero")]
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public Order()
        {
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Created;
        }

        public Order(string customerName, decimal totalAmount) : this()
        {
            CustomerName = customerName;
            TotalAmount = totalAmount;
        }

        // TODO: Refatorar validação para usar FluentValidation no futuro
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(CustomerName) && 
                   CustomerName.Length <= 100 && 
                   TotalAmount > 0;
        }

        // Regras de negócio para status
        public bool CanBeCancelled() => Status == OrderStatus.Created || Status == OrderStatus.Paid;
        public bool CanBePaid() => Status == OrderStatus.Created;
        public bool CanBeShipped() => Status == OrderStatus.Paid;
        public bool CanBeDeleted() => Status == OrderStatus.Created; // Só pode deletar se ainda não foi pago
    }
} 