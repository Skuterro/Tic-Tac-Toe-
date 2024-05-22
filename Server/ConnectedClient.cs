using System.Net.Sockets;


namespace server
{
    internal class ConnectedClient
    {
        public string PlayerName { get; set; }

        public TcpClient Client { get; set; }
    }
}
