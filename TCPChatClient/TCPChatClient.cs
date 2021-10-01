using System;
using System.Net;
using System.Net.Sockets;


namespace TCPChatClient {

    public class TCPChatClient {


        public static string defaultIP = "145.107.110.146";
        public static int defaultPort = 8900;
        public static int defaultID = 1;

        public static TCP tcp;

        public static void Main() {

            Console.Title = "TCP Chat Client Demo";

            tcp = new TCP(defaultID);

            Console.WriteLine("Trying to connect to server on port: " + defaultPort);
            tcp.Connect(defaultIP, defaultPort);

            Console.Read();
        }
    }
}