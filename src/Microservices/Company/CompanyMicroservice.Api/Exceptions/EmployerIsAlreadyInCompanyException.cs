namespace CompanyMicroservice.Api.Exceptions
{
    public class EmployerIsAlreadyInCompanyException : Exception
    {
        public EmployerIsAlreadyInCompanyException() { }
        public EmployerIsAlreadyInCompanyException(string message) : base(message) { }
    }
}
