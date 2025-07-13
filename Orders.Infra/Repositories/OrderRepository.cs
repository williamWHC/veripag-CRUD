using Orders.Domain.Entities;

namespace Orders.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private static readonly Dictionary<int, Order> _orders = new();
        private static readonly object _lock = new();
        private static int _nextId = 1;

        public Task<IEnumerable<Order>> GetAllAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_orders.Values.AsEnumerable());
            }
        }

        public Task<Order?> GetByIdAsync(int id)
        {
            lock (_lock)
            {
                _orders.TryGetValue(id, out var order);
                return Task.FromResult(order);
            }
        }

        public Task<Order> CreateAsync(Order order)
        {
            lock (_lock)
            {
                order.Id = _nextId++;
                _orders[order.Id] = order;
                return Task.FromResult(order);
            }
        }

        public Task<Order?> UpdateAsync(int id, Order order)
        {
            lock (_lock)
            {
                if (!_orders.ContainsKey(id))
                    return Task.FromResult<Order?>(null);
                order.Id = id;
                _orders[id] = order;
                return Task.FromResult<Order?>(order);
            }
        }

        public Task<bool> DeleteAsync(int id)
        {
            lock (_lock)
            {
                return Task.FromResult(_orders.Remove(id));
            }
        }
    }
} 