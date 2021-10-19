using System;

namespace TCPChatClient {
    class TCPClientHandle {


        // Confirm that the welcome message has been received (and display)
        public static void WelcomeReturn(Packet packet) {


            string welcomeMSG = packet.PacketReadString(true);

            int thisClientID = packet.PacketReadInt(false);
            
            Funcs.printMessage(3, welcomeMSG, false);
            TCPChatClient.clientID = thisClientID;
            TCPClientSend.ReceivedWelcome();
        }


        public static void DisplayMessage(Packet packet) {

            string message = packet.PacketReadString(true);

            Funcs.printMessage(3, message, false);
        }


        public static void DisplayChat(Packet packet) {

            string message = packet.PacketReadString(true);

            Funcs.printMessage(4, message, false);
        }


        public static void DisplayConnected(Packet packet) {

            string userName = packet.PacketReadString(true);

            Funcs.printMessage(3, $"{userName} has connected to this chat room!", false);
        }



        public static void DisplayDisconnected(Packet packet) {

            string userName = packet.PacketReadString(true);

            Funcs.printMessage(3, $"{userName} has disconnected from this chat room!", false);
        }
    }
}