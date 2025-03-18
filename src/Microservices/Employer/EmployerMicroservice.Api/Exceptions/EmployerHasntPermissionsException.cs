namespace EmployerMicroservice.Api.Exceptions
{
    public class EmployerHasntPermissionsException : Exception
    {
        public EmployerHasntPermissionsException() { }
        public EmployerHasntPermissionsException(string message) : base(message) { }
    }
}
