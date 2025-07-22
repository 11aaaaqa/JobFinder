namespace Web.MVC.Services.Hub_connection_services
{
    public interface IHubConnectionsManager
    {
        public void AddConnection(string userEmail, string hubConnection);
        public string? GetConnection(string userEmail);
        public void RemoveConnection(string userEmail);
    }
}
