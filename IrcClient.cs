using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace TwitchChatReader
{
    class IrcClient
    {
        string UserName;
        string Channel;

        TcpClient client;
        StreamReader input;
        StreamWriter output;
        public IrcClient(string ip, int port, string userName, string password)
        {

            UserName = userName;
            client = new TcpClient(ip, port);
            input = new StreamReader(client.GetStream());
            output = new StreamWriter(client.GetStream());

            output.WriteLine("PASS " + password);
            output.WriteLine("NICK " + userName);
            output.WriteLine("USER " + userName + " 8 * :" + userName);
            output.Flush();
        }

        public void JoinRoom(string channel)
        {
            Channel = channel;
            output.WriteLine("JOIN #" + channel);
            output.Flush();
        }

        public void SendIRCMessage(string message)
        {
            output.WriteLine(message);
            output.Flush();
        }

        public void SendChatMessage(string message)
        {
            SendIRCMessage(":" + UserName + "!" + 
                UserName + "@" + UserName + 
                ".tmi.twitch.tv PRIVMSG #" + Channel + " :" + message);
        }

        public string readMessage()
        {
            string message = input.ReadLine().Split('#').Last().Split(':').Last();
            return message;
    
        }
    }
}
