using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TCPChatClient {
    public class TCP {

        
        public TcpClient socket;    // Information stored that gets saved in server in the callback method
        private readonly int id;

        private NetworkStream stream;
        private byte[] receiveByteArray;

        public static int dataBufferSize = 4096;

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

            if(!socket.Connected) {
                return;
            }

            stream = socket.GetStream();
            stream.BeginRead(receiveByteArray, 0, dataBufferSize, StreamReceiveCallback, null);

            Console.WriteLine("Connected to server on port: " + TCPChatClient.defaultPort);
        }


        void StreamReceiveCallback(IAsyncResult aResult) {

            try {
                socket.EndConnect(aResult);

                if (!socket.Connected) {
                    return;
                }

                stream = socket.GetStream();
                stream.BeginRead(receiveByteArray, 0, dataBufferSize, StreamReceiveCallback, null);
            } catch {

                Console.WriteLine("Error! ==> disconnecting");
            }
        }
    }
}