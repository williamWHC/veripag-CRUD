namespace Orders.Application.DTOs
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        
        // Metadados adicionais para boas práticas
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? RequestId { get; set; }
        
        public static ApiResponse<T> SuccessResult(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }
        
        public static ApiResponse<T> ErrorResult(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default,
                Timestamp = DateTime.UtcNow
            };
        }
    }
} 