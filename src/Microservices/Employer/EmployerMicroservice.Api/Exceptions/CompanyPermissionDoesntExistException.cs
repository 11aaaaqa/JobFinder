namespace EmployerMicroservice.Api.Exceptions
{
    public class CompanyPermissionDoesntExistException : Exception
    {
        public CompanyPermissionDoesntExistException() { }
        public CompanyPermissionDoesntExistException(string message) : base(message) { }
    }
}
