using System;

namespace TCPChatClient {
    class TCPClientHandle {


        // Confirm that the welcome message has been received (and displayed)
        public static void WelcomeReturn(Packet packet) {

            string welcomeMSG = packet.PacketReadString(true);
            int thisClientID = packet.PacketReadInt(true);

            Funcs.printMessage(3, welcomeMSG, false);
            TCPChatClient.clientID = thisClientID;
        }
    }
}