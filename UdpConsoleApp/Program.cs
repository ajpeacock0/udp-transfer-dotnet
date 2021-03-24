using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace UdpConsoleApp
{
    class Program
    {
        private static readonly string SERVER_IP = "127.0.0.1";
        private static readonly int UDP_LISTEN_PORT = 11000;

        static int Main(string[] args)
        {
            // Create a root command with some options
            var rootCommand = new RootCommand { };

            rootCommand.Description = "Console app for UDP Client";

            rootCommand.Add(StartUdpClient());

            // Parse the incoming args and invoke the handler
            return rootCommand.InvokeAsync(args).Result;
        }

        /// <summary>
        /// Basic UDP data transfer using a UDP client
        /// </summary>
        private static Command StartUdpClient()
        {
            var command = new Command("udp");
            var serverOption = new Option(new[] { "--server", "-s" }, "Run just the server side.");
            var clientOption = new Option(new[] { "--client", "-c" }, "Run just the client side.");

            command.AddOption(serverOption);
            command.AddOption(clientOption);

            command.Handler = CommandHandler.Create<bool, bool>(async (bool server, bool client) =>
            {
                Console.WriteLine("----------------------");

                if (server ^ client)
                {
                    if (server)
                    {
                        Console.WriteLine("Starting Server");
                        await UdpTransport.ServerAsync(UDP_LISTEN_PORT);
                    }

                    if (client)
                    {
                        Console.WriteLine("Starting Client");
                        await UdpTransport.ClientAsync(SERVER_IP, UDP_LISTEN_PORT);
                    }
                }
                // If nothing or both were provided, perform self connection
                else
                {
                    Console.WriteLine("Starting self message");
                    UdpTransport.SelfMessage(SERVER_IP, UDP_LISTEN_PORT);
                }

                Console.WriteLine("----------------------");
            });

            return command;
        }
    }
}
