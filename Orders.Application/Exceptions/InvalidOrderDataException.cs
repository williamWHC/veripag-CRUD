namespace Orders.Application.Exceptions
{
    public class InvalidOrderDataException : ArgumentException
    {
        public InvalidOrderDataException(string message) : base(message)
        {
        }

        public InvalidOrderDataException(string message, string paramName) : base(message, paramName)
        {
        }
    }
} 