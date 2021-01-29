using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HW_Monitor_Server
{
    class Server
    {
        private HWMonitor hwmonitor = new HWMonitor();
        public void start()
        {
            byte[] data;
            byte[] reply;

            UdpClient socket = new UdpClient(16779);

            while (true)
            {
                Console.WriteLine("Waiting for a client...");
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 16779);
                data = socket.Receive(ref remoteEP);

                Console.WriteLine("Message received from {0}", remoteEP.ToString());
                if (Encoding.ASCII.GetString(data, 0, data.Length) == "S")
                {
                    reply = Encoding.ASCII.GetBytes(hwmonitor.GetStaticInfo());
                } else
                {
                    reply = Encoding.ASCII.GetBytes(hwmonitor.GetDynamicInfo());
                }

                socket.Send(reply, reply.Length, remoteEP);
                Console.WriteLine(String.Format("Data sent:\n {0}", Encoding.ASCII.GetString(reply)));
                Console.WriteLine();
            }
        }

    }
}
