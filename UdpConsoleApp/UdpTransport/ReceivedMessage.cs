using System.Net;

namespace UdpConsoleApp
{
    public struct ReceivedMessage
    {
        public IPEndPoint Sender;
        public string Message;
    }
}
