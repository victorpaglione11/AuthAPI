namespace Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        public abstract int StatusCode { get; }
        protected BaseException(string message) : base(message) { }
    }
}
