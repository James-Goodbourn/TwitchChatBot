using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace TwitchChatReader
{
    class Program
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        static System.Timers.Timer timer;
        static IrcClient irc;

        static bool HasInput = false;
        static void Main(string[] args)
        {
            irc = new IrcClient("irc.twitch.tv", 6667, "enchebot", "oauth:rtwexvheqi1yz7aupnhuxy2cfnu3n4");
            irc.JoinRoom("jimyfastfingers");

            timer = new System.Timers.Timer(300000);
            timer.Elapsed += timer_Tick;
            timer.Enabled = true;

            while (true)
            {
                try
                {
                    string message = irc.readMessage();
                    if (!HasInput)
                    {
                        if (message != null)
                        {
                            Console.WriteLine(message);
                            if (message.ToLower() == "up")
                            {
                                ProcessInput(VirtualKeyCode.VK_W);
                            }
                            if (message.ToLower() == "down")
                            {
                                ProcessInput(VirtualKeyCode.VK_S);
                            }
                            if (message.ToLower() == "left")
                            {
                                ProcessInput(VirtualKeyCode.VK_A);
                            }
                            if (message.ToLower() == "right")
                            {
                                ProcessInput(VirtualKeyCode.VK_D);
                            }
                            if (message.ToLower() == "a")
                            {
                                ProcessInput(VirtualKeyCode.VK_E);
                            }
                            if (message.ToLower() == "b")
                            {
                                ProcessInput(VirtualKeyCode.BACK);
                            }
                            if (message.ToLower() == "start")
                            {
                                ProcessInput(VirtualKeyCode.RETURN);
                            }
                            if (message.ToLower() == "lb")
                            {
                                ProcessInput(VirtualKeyCode.LEFT);
                            }
                            if (message.ToLower() == "rb")
                            {
                                ProcessInput(VirtualKeyCode.RIGHT);
                            }

                            if (message.ToLower() == "!cmd")
                            {
                                irc.SendChatMessage("left, right, up ,down, b, a, lb, rb, start");
                            }
                        }
                    }
                }
                catch
                { 
                    
                }
            }
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            irc.SendChatMessage("Commands: left, right, up ,down, b, a, lb, rb, start");
        }

        private static void ProcessInput(VirtualKeyCode Key)
        {
            Process p = Process.GetProcessesByName("Enché").FirstOrDefault();
            if (p != null)
            {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);
                InputSimulator x = new InputSimulator();
                x.Keyboard.KeyDown(Key);
                new Task(() =>
                {
                    System.Threading.Thread.Sleep(100);
                    x.Keyboard.KeyUp(Key);
                    HasInput = false;
                }).Start();
            }
        }
    }
}
