using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UdpConsoleApp
{
    abstract class UdpBase
    {
        protected UdpClient Client;

        protected UdpBase()
        {
            Client = new UdpClient();
        }

        public async Task<ReceivedMessage> ReceiveAsync()
        {
            var result = await Client.ReceiveAsync();
            return new ReceivedMessage()
            {
                Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
                Sender = result.RemoteEndPoint
            };
        }

        public string Receive(ref IPEndPoint? remoteEP)
        {
            var result = Client.Receive(ref remoteEP);
            return Encoding.ASCII.GetString(result);
        }

        public int Send(byte[] dgram, int bytes, IPEndPoint? endpoint)
        {
            return Client.Send(dgram, bytes, endpoint);
        }
    }
}
