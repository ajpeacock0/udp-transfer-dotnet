using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UdpConsoleApp
{
    class UdpTransport
    {
        public static async Task ServerAsync(int udpListenPort)
        {
            var udpServer = new UdpListener(udpListenPort);

            while (true)
            {
                var received = await udpServer.ReceiveAsync();

                Console.WriteLine("Received data \"" + received.Message + "\" from " + received.Sender.ToString());

                string sendingDataString = "pong";
                byte[] sendingDataRaw = Encoding.ASCII.GetBytes(sendingDataString);

                // send reply
                udpServer.Send(sendingDataRaw, sendingDataRaw.Length, received.Sender);

                Console.WriteLine("Sent data \"" + sendingDataString + "\" to " + received.Sender.ToString());
            }
        }

        public static async Task ClientAsync(string serverIp, int udpListenPort)
        {
            var client = UdpUser.ConnectTo(serverIp, udpListenPort);

            string sendingDataString = "ping";

            client.Send(sendingDataString);

            Console.WriteLine($"Sent data \"{sendingDataString}\" to {serverIp}:{udpListenPort}");

            // then receive data
            var received = await client.ReceiveAsync();

            Console.WriteLine("Received data \"" + received.Message + "\" from " + received.Sender.ToString());
        }

        public static void SelfMessage(string serverIp, int udpListenPort)
        {
            // Listen for messages and replay the messages back to the client
            var server = new UdpListener(udpListenPort);
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    var received = await server.ReceiveAsync();
                    Console.WriteLine("Server received data \"" + received.Message + "\" from " + received.Sender.ToString());

                    server.Reply($"Replayed message \"{received.Message}\"", received.Sender);
                    if (received.Message == "quit")
                        break;
                }
            });

            // Create a client connecting to Server
            var client = UdpUser.ConnectTo(serverIp, udpListenPort);

            // Print out any messages the client receives
            Task.Factory.StartNew(async () => {
                while (true)
                {
                    try
                    {
                        var received = await client.ReceiveAsync();
                        Console.WriteLine("Client received data \"" + received.Message + "\" from " + received.Sender.ToString());

                        if (received.Message.Contains("quit"))
                            break;
                    }
                    catch (Exception ex)
                    {
                        Debug.Write(ex);
                    }
                }
            });

            // Read data from console and send to server
            string read;
            do
            {
                read = Console.ReadLine();
                client.Send(read);
            } while (read != "quit");
        }
    }
}
