﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TCPChatClient {
    public static class Funcs {

        private static string errorAlert = "[ERROR] ";
        private static string warningAlert = "[WARNING] ";
        private static string messageAlert = "[MSG] ";


        public static void printMessage(int alertLevel, string message, bool typeWrite) {

            switch (alertLevel) {
                case 0:
                    
                    string msgErr = errorAlert + "{" + DateTime.Now.ToString("HH:mm:ss") + "} " + message;

                    if (typeWrite)
                        slowType(msgErr, 3);
                    else
                        Console.WriteLine(msgErr);
                    
                    break;

                case 1:
                    string msgWarn = warningAlert + "{" + DateTime.Now.ToString("HH:mm:ss") + "} " + message;

                    if (typeWrite)
                        slowType(msgWarn, 3);
                    else
                        Console.WriteLine(msgWarn);

                    break;

                case 2:
                    string msgMsg = messageAlert + "{" + DateTime.Now.ToString("HH:mm:ss") + "} " + message;
                    
                    if (typeWrite)
                        slowType(msgMsg, 3);
                    else
                        Console.WriteLine(msgMsg);

                    break;

                default:
                    break;
            }
        }


        public static void slowType(string message, int delay) {
            foreach (char character in message) {
                Console.Write(character);
                System.Threading.Thread.Sleep(delay);
            }
            Console.Write("\n");
        }
    }
}