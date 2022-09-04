using System.Collections.Generic;
using System.Threading;
using Xunit;

namespace Sandbox.Tests
{
    public class ServerClientTests
    {
        [Fact]
        public void Client_Disconnect_MultipleDisconnects()
        {
            var server = new Server();
            var client1 = new Client(server);
            var client2 = new Client(server);

            client1.Disconnect();
            client2.Disconnect();
            client1.Disconnect();

            Assert.False(client1.AmIConnectedToServer());
            Assert.False(client2.AmIConnectedToServer());
        }
    }

    public class Server
    {
        private readonly List<Client> _clients = new List<Client>();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public void AddClient(Client client)
        {
            _clients.Add(client);
        }
        public void RemoveClient(Client client)
        {
            _semaphore.Wait();
            try
            {
                if (_clients.Contains(client))
                {
                    _clients.Remove(client);
                }
            }
            finally
            {
                _semaphore.Release();
            }
        }
        public bool IsClientInList(Client client)
        {
            return _clients.IndexOf(client) != -1;
        }
    }

    public class Client
    {
        private readonly Server _parent;

        public Client(Server parent)
        {
            _parent = parent;
        }


        public void Disconnect()
        {
            _parent.RemoveClient(this);
        }

        public bool AmIConnectedToServer()
        {
            return _parent.IsClientInList(this);
        }
    }
}