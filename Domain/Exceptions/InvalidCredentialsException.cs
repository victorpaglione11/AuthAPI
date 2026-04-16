namespace Domain.Exceptions
{
    public class InvalidCredentialsException : BaseException
    {
        public override int StatusCode => 401;
        public InvalidCredentialsException() : base("Usuário ou senha inválidos.") { }
    }
}
