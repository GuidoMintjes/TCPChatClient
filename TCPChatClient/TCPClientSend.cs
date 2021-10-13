using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TCPChatClient {
    class TCPClientSend {
    
        // Acknowledge the welcome message and send back client ID as confirmation and username
        public static void ReceivedWelcome() {

            Packet packet = new Packet((int) ClientPackets.welcomeReceived);

            packet.PacketWrite(TCPChatClient.clientID);
            packet.PacketWrite(TCPChatClient.userName);
            
            TCPSendData(packet);

            Funcs.printMessage(2, $"Sent welcome received packet with username {TCPChatClient.userName} and this is ID {TCPChatClient.clientID}",
                                false);
        }

        
        // Method to call to start to send an actual packet
        public static void TCPSendData(Packet packet) {

            packet.PacketWriteLength();
            TCPChatClient.tcp.TCPSendData(packet);
        }
    }
}