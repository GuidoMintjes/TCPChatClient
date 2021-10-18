using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TCPChatClient {
    public static class NetworkCommand {


        public static void CommandLoop() {

            string commandRaw;

            while (true) {
                if (Console.KeyAvailable) {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key) {
                        case ConsoleKey.Enter:
                            Console.WriteLine();
                            Funcs.printMessage(4, "Input chat message:", false);
                            commandRaw = Console.ReadLine();
                            Console.WriteLine();

                            SendMessage("<" + TCPChatClient.userName + "> " + commandRaw);
                            break;
                        default:
                            break;
                    }
                }
            }
        }



        private static void SendMessage(string commandRaw) {

            TCPClientSend.TCPSendData(CreateMessagePacket(commandRaw));
            CommandLoop();
        }


        private static Packet CreateMessagePacket(string message) {

            Packet packet = new Packet((int)ServerPackets.chat);

            packet.PacketWrite(message);

            return packet;
        }
    }
}