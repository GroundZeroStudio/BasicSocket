using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EchoClient
{
    class Program
    {
        static Socket mSocket;
        static byte[] mReadBuff = new byte[1024];
        static string mReceiveMsg = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Connection();
            Console.ReadLine();
        }

        static void Connection()
        {
            // 新建
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 建立连接
            mSocket.BeginConnect("127.0.0.1", 8888, ConnectionCallBack, mSocket);
        }

        static void ConnectionCallBack(IAsyncResult rResult)
        {
            try
            {
                Socket rSocket = (Socket)rResult.AsyncState;
                rSocket.EndConnect(rResult);
                Console.WriteLine("socket connect success");
                rSocket.BeginReceive(mReadBuff, 0, 1024, 0, ReceiveCallBack, rSocket);
                Send("测试服务器连接1"); 
                Send("测试服务器连接2");
            }
            catch
            {
                Console.WriteLine("socket connect fail");
            }
        }

        static void Send(string rString)
        {
            byte[] rSendBytes = System.Text.Encoding.Default.GetBytes(rString);
            
            // 发送
            mSocket.BeginSend(rSendBytes, 0, rSendBytes.Length, 0, SendCallBack, mSocket);
            
            // 关闭
            //mSocket.Close();
        }

        static void SendCallBack(IAsyncResult rResult)
        {
            try
            {
                Socket rSocket = (Socket)rResult.AsyncState;
                int nCount = rSocket.EndSend(rResult);
                Console.WriteLine("Send Success");
            }
            catch
            {
                Console.WriteLine("Send fail");
            }
        }

        static void ReceiveCallBack(IAsyncResult rResult)   
        {
            try
            {
                Socket rSocket = (Socket)rResult.AsyncState;
                int nCount = rSocket.EndReceive(rResult);
                mReceiveMsg = System.Text.Encoding.Default.GetString(mReadBuff, 0, nCount);
                rSocket.BeginReceive(mReadBuff, 0, 1024, 0, ReceiveCallBack, rSocket);
                Console.WriteLine(mReceiveMsg);
            }
            catch
            {
                Console.WriteLine("Receive fail");
            }
        }
    }
}
