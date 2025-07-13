# Orders API

API para gerenciamento de pedidos desenvolvida em .NET 8 seguindo DDD.

## Funcionalidades

- **GET /orders** - Lista todos os pedidos
- **GET /orders/{id}** - Busca pedido específico
- **POST /orders** - Cria novo pedido
- **PUT /orders/{id}** - Atualiza pedido
- **DELETE /orders/{id}** - Remove pedido

## Estrutura

```
Orders/
├── Orders.API/          # Controllers
├── Orders.Application/   # DTOs, Interfaces
├── Orders.Domain/        # Entities, Enums
└── Orders.Infra/         # Repositories, Services
```

## Como Executar

1. Clone o repositório
2. Navegue até a pasta: `cd Orders`
3. Execute: `dotnet run --project Orders.API`
4. Acesse: `http://localhost:5254`

## Modelo de Dados

### Order
```json
{
  "id": "guid",
  "customerName": "string",
  "orderDate": "datetime",
  "totalAmount": "decimal",
  "status": "Created|Paid|Shipped|Cancelled"
}
```

### CreateOrderDto
```json
{
  "customerName": "string",
  "totalAmount": "decimal"
}
```

### UpdateOrderDto
```json
{
  "customerName": "string",
  "totalAmount": "decimal",
  "status": "Created|Paid|Shipped|Cancelled"
}
```

## Validações

- **CustomerName**: Obrigatório, 2-100 caracteres
- **TotalAmount**: Obrigatório, > 0
- **Status**: Obrigatório para update

## Observações

- Armazenamento em memória (dados perdidos ao reiniciar)
- Pronto para expansão com banco de dados
- Respostas padronizadas com status e mensagens
