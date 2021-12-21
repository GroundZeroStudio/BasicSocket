using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace EchoServer
{
    class Program
    {
        static Socket mListenSocket;
        static Dictionary<Socket, ClientState> mClients = new Dictionary<Socket, ClientState>();

        static void Main(string[] args)
        {
            Console.WriteLine("Echo Server Enter");

            // 创建socket
            mListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPAddress rIpAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint rIpEndPoint = new IPEndPoint(rIpAdr, 8888);

            // 绑定地址和端口
            mListenSocket.Bind(rIpEndPoint);

            // 监听
            mListenSocket.Listen(0);
            Console.WriteLine("服务器启动成功");
            mListenSocket.BeginAccept(AcceptCallback, mListenSocket);
            Console.ReadLine();
        }

        static void AcceptCallback(IAsyncResult rResult)
        {
            Console.WriteLine("服务器Accept");

            Socket rListenSocket = (Socket)rResult.AsyncState;
            Socket rClientSocket = rListenSocket.EndAccept(rResult);
            ClientState rClientState = new ClientState();
            rClientState.mSocket = rClientSocket;
            mClients.Add(rClientSocket, rClientState);
            rClientSocket.BeginReceive(rClientState.readBuff, 0, 1024, SocketFlags.None, ReceiveCallback, rClientState);
            rListenSocket.BeginAccept(AcceptCallback, rListenSocket);

        }

        static void ReceiveCallback(IAsyncResult rResult)
        {
            ClientState rClientState = (ClientState)rResult.AsyncState;
            Socket rClientSocket = rClientState.mSocket;
            int nCount = rClientState.mSocket.EndReceive(rResult);
        
            if(nCount == 0)
            {
                rClientSocket.Close();
                mClients.Remove(rClientSocket);
                Console.WriteLine("Socket Close");
                return;
            }

            string rReadStr = System.Text.Encoding.Default.GetString(rClientState.readBuff, 0, nCount);

            Console.WriteLine("服务器接收" + rReadStr);
            byte[] rSendBytes = System.Text.Encoding.Default.GetBytes(rReadStr);
            rClientSocket.Send(rSendBytes);
        }
    }

    class ClientState
    {
        public Socket mSocket;
        public byte[] readBuff = new byte[1024];
    }
}
