using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace TCPChatClient {

    public class TCPChatClient {


        private static bool isRunning = false;

        public static TCPChatClient instance;

        
        public static string connectIP;
        public static int connectPort = 8900;
        public static int dataBufferSize = 4096;
        public static TCP tcp;


        public static string userName = @"{userName}";


        private static Dictionary<int, PacketHandler> packetHandlers;
        private delegate void PacketHandler(Packet packet);


        public static int clientID = 0;
        
        public static int countIntegers = 0;


        #region Start Program

        public static void Main() {

            Console.Title = "TCP Chat Client Demo";

            tcp = new TCP();

            Funcs.printMessage(3, "What is your desired username?", true);
            string name = Console.ReadLine();
            userName = name;


            Funcs.printMessage(3, "What is the ip of the server you are trying to connect to?", true);
            string ip = Console.ReadLine();
            connectIP = ip;


            Funcs.printMessage(3, "Trying to connect to server on port: " + connectPort, true);
            tcp.Connect();

            ThreadManager.UpdateMain();

            isRunning = true;           // Set running status to be active

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
        }


        // Run the logic loop
        private static void MainThread() {

            Funcs.printMessage(1, $"Main thread with loop started at {Consts.TICKMSDURATION} ms per tick!", false);
            DateTime nextCycle = DateTime.Now;

            while (isRunning) {

                while (nextCycle < DateTime.Now) {

                    Logic.Update();

                    nextCycle = nextCycle.AddMilliseconds(Consts.TICKMSDURATION);

                    // Fix voor hoge CPU usage
                    if (nextCycle > DateTime.Now) {

                        Thread.Sleep(nextCycle - DateTime.Now);
                    }
                }
            }
        }

        #endregion

        #region TCP Class etc

        public class TCP {


            public TcpClient socket;    // Information stored that gets saved in server in the callback method

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveByteArray;


            public void Connect() {

                InitializePacketHandlers();

                socket = new TcpClient {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };


                receiveByteArray = new byte[dataBufferSize];    // Gets the 'stream' of info provided by the socket

                socket.BeginConnect(connectIP, connectPort, SocketConnectCallback, socket);

                NetworkCommand.CommandLoop();
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

                Funcs.printMessage(3, "Connected to server on port: " + TCPChatClient.connectPort, true);
            }


            // Handle stream and socket when the stream is received
            void StreamReceiveCallback(IAsyncResult aResult) {

                try {

                    //socket.EndConnect(aResult);

                    if (!socket.Connected) {
                        return;
                    }

                    int dataArrayLength = stream.EndRead(aResult);
                    byte[] dataArray = new byte[dataArrayLength];

                    Array.Copy(receiveByteArray, dataArray, dataArrayLength);

                    stream = socket.GetStream();

                    receivedData.NullifyPacket(HandleData(dataArray));

                    stream.BeginRead(receiveByteArray, 0, dataBufferSize, StreamReceiveCallback, null);

                } catch(Exception exc) {

                    Funcs.printMessage(0, "Error! ==> disconnecting " + exc, false);
                }
            }


            // Handles the data and returns a boolean, this is needed because we might not want to always reset the pack
            private bool HandleData(byte[] dataArray) {

                int packetLength = 0;

                receivedData.SetPacketBytes(dataArray); // Load the data into the Packet instance

                /*
                foreach (byte byt in dataArray) {
                    Console.Write(Convert.ToString(byt, 2).PadLeft(8, '0'));
                    Console.Write(" ");
                }

                Console.WriteLine();
                */


                // Check if what still needs to be read is an integer or bigger, if so that is the first int of the packet indicating
                // the length of that packet
                if (receivedData.GetUnreadPacketSize() >= 4) {

                    packetLength = receivedData.PacketReadInt(true);

                    Console.WriteLine("Got packet length of: " + packetLength.ToString());

                    // Check if packet size is 0 or less, if so, return true so that the packet will be reset
                    if (packetLength <= 0) {

                        return true;
                    }
                }


                // While this is true there is still data that needs to be handled
                while (packetLength > 0 && packetLength <= receivedData.GetUnreadPacketSize()) {

                    byte[] packetBytes = receivedData.PacketReadBytes(packetLength, true);

                    //ThreadManager.ExecuteOnMainThread(() => {

                        using (Packet packet = new Packet()) {

                            packet.SetPacketBytes(packetBytes);

                            int packetID = packet.PacketReadInt(true);

                            packetHandlers[packetID](packet);
                        }
                    //});

                    packetLength = 0;

                    if (receivedData.GetUnreadPacketSize() >= 4) {

                        packetLength = receivedData.PacketReadInt(false);

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

                    { (int) ServerPackets.welcome, TCPClientHandle.WelcomeReturn },
                    { (int) ServerPackets.message, TCPClientHandle.DisplayMessage },
                    { (int) ServerPackets.chat, TCPClientHandle.DisplayChat }
                };

                Funcs.printMessage(2, "Packet handler dictionary initiated!", true);
            }


            public void TCPSendData(Packet packet) {

                try {

                    if (socket != null) {

                        stream.BeginWrite(packet.GetPacketBytes(), 0, packet.GetPacketSize(), null, null);
                    }

                } catch (Exception exc) {

                    Funcs.printMessage(0, $"Unable to send data to server through TCP, err msg: {exc}", false);
                }
            }
        }
    }
}


#endregion