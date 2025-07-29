namespace Web.MVC.Services.Hub_connection_services
{
    public class HubConnectionsManager : IHubConnectionsManager
    {
        private static readonly Dictionary<string, string> hubConnections = new();

        public void AddConnection(string userEmail, string hubConnection)
        {
            var result = hubConnections.TryAdd(userEmail, hubConnection);
            if(!result)
                hubConnections[userEmail] = hubConnection;
        }

        public string? GetConnection(string userEmail)
            => hubConnections.SingleOrDefault(x => x.Key == userEmail).Value;

        public void RemoveConnection(string userEmail)
        {
            hubConnections.Remove(userEmail);
        }
    }
}
