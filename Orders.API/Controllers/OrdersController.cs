using Microsoft.AspNetCore.Mvc;
using Orders.Application.DTOs;
using Orders.Application.Interfaces;
using Orders.Application.Exceptions;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("orders")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Retorna a lista de todos os pedidos cadastrados no sistema
        /// </summary>
        /// <returns>Lista de pedidos com informações completas</returns>
        /// <response code="200">Lista de pedidos recuperada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<OrderDto>>), 500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<OrderDto>>>> GetOrders()
        {
            _logger.LogInformation("Iniciando busca de todos os pedidos");
            var result = await _orderService.GetAllOrdersAsync();
            result.RequestId = HttpContext.TraceIdentifier;
            return Ok(result);
        }

        /// <summary>
        /// Retorna um pedido específico pelo ID
        /// </summary>
        /// <param name="id">Identificador único do pedido (número sequencial)</param>
        /// <returns>Detalhes do pedido solicitado</returns>
        /// <response code="200">Pedido encontrado com sucesso</response>
        /// <response code="404">Pedido não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetOrder(int id)
        {
            _logger.LogInformation("Buscando pedido com ID: {OrderId}", id);
            var result = await _orderService.GetOrderByIdAsync(id);
            result.RequestId = HttpContext.TraceIdentifier;
            return Ok(result);
        }

        /// <summary>
        /// Cria um novo pedido no sistema
        /// </summary>
        /// <param name="createOrderDto">Dados do pedido a ser criado</param>
        /// <returns>Pedido criado com ID gerado automaticamente</returns>
        /// <response code="201">Pedido criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            _logger.LogInformation("Criando novo pedido para cliente: {CustomerName}", createOrderDto.CustomerName);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new InvalidOrderDataException($"Dados inválidos: {string.Join(", ", errors)}");
            }

            var result = await _orderService.CreateOrderAsync(createOrderDto);
            result.RequestId = HttpContext.TraceIdentifier;
            return CreatedAtAction(nameof(GetOrder), new { id = result.Data!.Id }, result);
        }

        /// <summary>
        /// Atualiza um pedido existente pelo ID
        /// </summary>
        /// <param name="id">Identificador único do pedido (número sequencial)</param>
        /// <param name="updateOrderDto">Dados atualizados do pedido</param>
        /// <returns>Pedido atualizado com as novas informações</returns>
        /// <response code="200">Pedido atualizado com sucesso</response>
        /// <response code="400">Dados inválidos ou transição de status inválida</response>
        /// <response code="404">Pedido não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 400)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 404)]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), 500)]
        public async Task<ActionResult<ApiResponse<OrderDto>>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            _logger.LogInformation("Atualizando pedido com ID: {OrderId}", id);
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                throw new InvalidOrderDataException($"Dados inválidos: {string.Join(", ", errors)}");
            }

            var result = await _orderService.UpdateOrderAsync(id, updateOrderDto);
            result.RequestId = HttpContext.TraceIdentifier;
            return Ok(result);
        }

        /// <summary>
        /// Remove um pedido pelo ID
        /// </summary>
        /// <param name="id">Identificador único do pedido (número sequencial)</param>
        /// <returns>Confirmação da exclusão</returns>
        /// <response code="200">Pedido excluído com sucesso</response>
        /// <response code="400">Pedido não pode ser excluído devido ao status</response>
        /// <response code="404">Pedido não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 400)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 404)]
        [ProducesResponseType(typeof(ApiResponse<bool>), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteOrder(int id)
        {
            _logger.LogInformation("Excluindo pedido com ID: {OrderId}", id);
            var result = await _orderService.DeleteOrderAsync(id);
            result.RequestId = HttpContext.TraceIdentifier;
            return Ok(result);
        }
    }
} 