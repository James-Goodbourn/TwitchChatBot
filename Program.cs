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
                        HasInput = true;
                        var messages = message.Split(' ');
                        Task.Run(() =>
                        {
                            Console.WriteLine(message);
                            var index = 0;
                            foreach (var item in messages)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    if (item.ToLower() == "up")
                                    {
                                        ProcessInput(VirtualKeyCode.VK_W);
                                    }
                                    if (item.ToLower() == "down")
                                    {
                                        ProcessInput(VirtualKeyCode.VK_S);
                                    }
                                    if (item.ToLower() == "left")
                                    {
                                        ProcessInput(VirtualKeyCode.VK_A);
                                    }
                                    if (item.ToLower() == "right")
                                    {
                                        ProcessInput(VirtualKeyCode.VK_D);
                                    }
                                    if (item.ToLower() == "a")
                                    {
                                        ProcessInput(VirtualKeyCode.VK_E);
                                    }
                                    if (item.ToLower() == "b")
                                    {
                                        ProcessInput(VirtualKeyCode.BACK);
                                    }
                                    if (item.ToLower() == "start")
                                    {
                                        ProcessInput(VirtualKeyCode.RETURN);
                                    }
                                    if (item.ToLower() == "lb")
                                    {
                                        ProcessInput(VirtualKeyCode.LEFT);
                                    }
                                    if (item.ToLower() == "rb")
                                    {
                                        ProcessInput(VirtualKeyCode.RIGHT);
                                    }

                                    if (item.ToLower() == "!cmd")
                                    {
                                        irc.SendChatMessage("left, right, up ,down, b, a, lb, rb, start");
                                    }
                                    System.Threading.Thread.Sleep(500);
                                    index++;
                                    if(index >= messages.Length)
                                    {
                                        HasInput = false;
                                    }
                                }
                            }
                        });
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
                }).Start();
            }
        }
    }
}
