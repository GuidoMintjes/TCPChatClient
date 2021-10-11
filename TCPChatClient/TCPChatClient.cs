using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;


namespace TCPChatClient {

    public class TCPChatClient {

        public static TCPChatClient instance;

        
        public static string defaultIP = "172.28.192.1";
        public static int dataBufferSize = 4096;
        public static int defaultPort = 8900;
        public static TCP tcp;


        private static Dictionary<int, PacketHandler> packetHandlers;
        private delegate void PacketHandler(Packet packet);


        public static int defaultID = 0;
        public int clientID = 0;
        

        public static int id;


        #region Start Program

        public static void Main() {

            Console.Title = "TCP Chat Client Demo";

            tcp = new TCP(defaultID);

            Console.WriteLine("What is the ip of the server you are trying to connect to?");
            string ip = Console.ReadLine();
            defaultIP = ip;

            
            Console.WriteLine("Trying to connect to server on port: " + defaultPort);
            tcp.Connect(defaultIP, defaultPort);

            ThreadManager.UpdateMainInvoke();

            Console.Read();
        }

        #endregion

        #region TCP Class etc

        public class TCP {


            public TcpClient socket;    // Information stored that gets saved in server in the callback method

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveByteArray;


            public TCP(int _id) {

                id = _id;
            }


            public void Connect(string ip, int port) {


                socket = new TcpClient {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };


                receiveByteArray = new byte[dataBufferSize];    // Gets the 'stream' of info provided by the socket

                socket.BeginConnect(ip, port, SocketConnectCallback, socket);
            }


            // Method that gets called back on when client connects to server
            void SocketConnectCallback(IAsyncResult aResult) {

                socket.EndConnect(aResult);

                if (!socket.Connected) {
                    return;
                }

                stream = socket.GetStream();

                receivedData = new Packet();

                stream.BeginRead(receiveByteArray, 0, dataBufferSize, StreamReceiveCallback, null);

                Console.WriteLine("Connected to server on port: " + TCPChatClient.defaultPort);
            }


            // Handle stream and socket when the stream is received
            void StreamReceiveCallback(IAsyncResult aResult) {

                try {

                    socket.EndConnect(aResult);

                    if (!socket.Connected) {
                        return;
                    }

                    int dataArrayLength = stream.EndRead(aResult);
                    byte[] dataArray = new byte[dataArrayLength];

                    Array.Copy(receiveByteArray, dataArray, dataArrayLength);

                    stream = socket.GetStream();

                    receivedData.NullifyPacket(HandleData(dataArray));

                    stream.BeginRead(receiveByteArray, 0, dataBufferSize, StreamReceiveCallback, null);

                } catch {

                    Console.WriteLine("Error! ==> disconnecting");
                }
            }


            // Handles the data and returns a boolean, this is needed because we might not want to always reset the pack
            private bool HandleData(byte[] dataArray) {

                int packetLength = 0;

                receivedData.SetPacketBytes(dataArray); // Load the data into the Packet instance


                // Check if what still needs to be read is an integer or bigger, if so that is the first int of the packet indicating
                // the length of that packet
                if (receivedData.GetUnreadPacketSize() >= 4) {

                    packetLength = receivedData.PacketReadInt(true);

                    // Check if packet size is 0 or less, if so, return true so that the packet will be reset
                    if (packetLength <= 0) {

                        return true;
                    }
                }


                // While this is true there is still data that needs to be handled
                while (packetLength > 0 && packetLength <= receivedData.GetUnreadPacketSize()) {

                    byte[] packetBytes = receivedData.PacketReadBytes(packetLength, true);

                    ThreadManager.ExecuteOnMainThread(() => {

                        Packet packet = new Packet();

                        int packetID = packet.PacketReadInt(true);

                        packetHandlers[packetID](packet);
                    });

                    packetLength = 0;

                    if (receivedData.GetUnreadPacketSize() >= 4) {

                        packetLength = receivedData.PacketReadInt(true);

                        // Check if packet size is 0 or less, if so, return true so that the packet will be reset
                        if (packetLength <= 0) {

                            return true;
                        }
                    }
                }


                if (packetLength <= 1) {
                    return true;
                }


                return false;       // In this case there is still a piece of data in the packet/stream which is part of some data
                                    // in some other upcoming packet, which is why it shouldn't be destroyed
            }

            private void InitializePacketHandlers() {

                packetHandlers = new Dictionary<int, PacketHandler>() {

                    { (int) ServerPackets.welcome, TCPClientHandle.WelcomeReturn }
                };
            }
        }

        #endregion
    }
}