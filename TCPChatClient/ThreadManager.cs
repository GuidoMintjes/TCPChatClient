using System;
using System.Timers;
using System.Collections.Generic;

namespace TCPChatClient {
    class ThreadManager {

        // An action is kind of like a thread, but it needs to be placed on a thread, in this case we only have the main thread
        private static readonly List<Action> executeOnMainThread = new List<Action>();
        private static readonly List<Action> executeCopiedOnMainThread = new List<Action>();
        private static bool actionToExecuteOnMainThread = false;

        private static double frameTime = 1000 / 60;


        // Set an Action on the main Thread
        public static void ExecuteOnMainThread(Action action) {
            
            if (action == null) {

                Funcs.printMessage(1, "Action could not be put on main thread because it's empty!", false);
                return;
            }

            lock (executeOnMainThread) {

                executeOnMainThread.Add(action);
                actionToExecuteOnMainThread = true;
            }
        }


        /// Executes the Actions/code on the main thread
        public static void UpdateMainInvoke() {
            

            Timer timer = new Timer(frameTime);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }


        // Actually run the Actions
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e) {

            Funcs.printMessage(2, "test", false);

            /*
            if (actionToExecuteOnMainThread) {

                executeCopiedOnMainThread.Clear();

                lock (executeOnMainThread) {

                    executeCopiedOnMainThread.AddRange(executeOnMainThread);
                    executeOnMainThread.Clear();
                    actionToExecuteOnMainThread = false;
                }

                for (int i = 0; i < executeCopiedOnMainThread.Count; i++) {

                    executeCopiedOnMainThread[i]();
                }
            }
            */
        }
    }
}