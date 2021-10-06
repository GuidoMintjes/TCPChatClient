using System;
using System.Net;
using System.Net.Sockets;


namespace TCPChatClient {

    public class TCPChatClient {


        public static string defaultIP = "172.28.192.1";
        public static int defaultPort = 8900;
        public static int defaultID = 1;

        public static TCP tcp;

        public static void Main() {

            Console.Title = "TCP Chat Client Demo";

            tcp = new TCP(defaultID);

            Console.WriteLine("What is the ip of the server you are trying to connect to?");
            string ip = Console.ReadLine();
            defaultIP = ip;

            Console.WriteLine("Trying to connect to server on port: " + defaultPort);
            tcp.Connect(defaultIP, defaultPort);

            Console.Read();
        }         
    }
}