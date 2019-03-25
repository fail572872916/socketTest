using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace socket_server
{
    delegate void asynServerRun();           //异步运行server服务的委托
    class server
    {
        Socket Server;
        string serverIP_PROT = "";

        delegate bool delehreatBeat(string client_hreatBeat);   //闭环心跳委托

        public void runServer()
        {
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            int prot = 30000;
            IPHostEntry ipe = Dns.GetHostEntry(Dns.GetHostName());                 //直接读取本机的IP地址
            string ip = ipe.AddressList[0].ToString();
            try
            {
                Server.Bind(new IPEndPoint(IPAddress.Any, prot));      //开始监听
                Server.Listen(20);
                serverIP_PROT = "服务打开成功\r\nIP:" + ip + ":" + prot + "\r\n";
                MessageBox.Show(serverIP_PROT);
            }
            catch
            {
                MessageBox.Show("网络服务打开失败，请重试");
            }

            asynConnect _asynConnect = monitor;
            _asynConnect.BeginInvoke(Server, null, null);
        }

        /// <summary>
        /// 侦听
        /// </summary>
        /// <param name="connect"></param>
        delegate void asynConnect(Socket connect);    //异步侦听的委托
     //   public static Dictionary<string, flag> client_list = new Dictionary<string, flag>();
        public static Dictionary<string, Socket> clientFlag_list = new Dictionary<string, Socket>();   //这个键集合是用Flag 当键索引client_list的键
        public byte[] buffer = new byte[1024];
        private void monitor(object o)
        {
            Socket listening = o as Socket;
         
            while (true)
            {
                try
                {
                    // 用内部connect套接字，传用server套接字
                    Socket connect = listening.Accept();
                    EndPoint clientIP = connect.RemoteEndPoint;
                    Form1.f1.addItems(clientIP.ToString());
                    Form1.f1.appendText(clientIP.ToString() + "已连接\r\n");

                    //flag flag = new flag();
                    //flag.socket = connect;
                  //  client_list.Add(clientIP.ToString(), flag);

                    //  _asynReceiveDate.BeginInvoke(connect, new AsyncCallback(receiveDate), connect);
                   
                    //  connect.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveMessage.OnReceiveCallback), connect);
                    StartKeepAlive StartKeepAlive = new StartKeepAlive(clientIP.ToString(), buffer);
                    StartKeepAlive._StartKeepAlive(connect);
                }
                catch { }
            }
        }


        /// <summary>
        /// 判断是否正常连接
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
       public  static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }
        /// <summary>
        /// 转发客户端消息，类似于QQ 点对点聊
        /// </summary>
        public static void onesend(string flag, string message)
        {  
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(message);      //将字符串转成byte数据       
            try
            {
                clientFlag_list[flag].Send(buffer, 0, buffer.Length, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Form1.f1.appendText(ex.Message +"222222"+ "\r\n");
            }
        }

        /// <summary>
        /// 转发客户端消息，类似于QQ 群聊
        /// </summary>
        public static void allsend(string message)
        {
            if (clientFlag_list.Count > 0)
            {

                for (int i = 0; i < clientFlag_list.Keys.Count; i++)
                {
                    // MessageBox.Show(client_list.Keys.ElementAt(i));
                    byte[] buffer = new byte[1024];
                    buffer = Encoding.UTF8.GetBytes(message);      //将字符串转成byte数据       
                    try
                    {
                        clientFlag_list[clientFlag_list.Keys.ElementAt(i)].Send(buffer, 0, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        Form1.f1.appendText(ex.Message +"11111111"+ "\r\n");
                    }
                }
            }
        }
        /// <summary>
        /// 断开所有SOCKET连接
        /// </summary>
        public void allClose()
        {
            foreach (Socket s in clientFlag_list.Values)
            {
                s.Shutdown(SocketShutdown.Both);
                s.Close();
            }
        }
        /// <summary>
        /// 下线操作
        /// </summary>
        /// <param name="clientUser">客户端的套接字</param>
        public static void downLine(string clientUser)
        {
            Form1.f1.RemoveItems(clientUser);
            Form1.f1.RemoveListbox_flag(clientUser);
            clientFlag_list.Remove(clientUser);
       //     client_list.Remove(clientUser);
            List<string> flaglist = new List<string>();
          
            allsend("11111");
        }
        /// <summary>
        /// 断开选定的SOCKET连接
        /// </summary>
        public void Close()
        {
            if (Form1.f1.listBox_flag.SelectedIndex != -1)               //判断是否选中了socket 对象
            {
                clientFlag_list[Form1.f1.listBox_flag.Text].Shutdown(SocketShutdown.Both);
            }
            else
            {
                MessageBox.Show("请选择断开对象");
            }
        }
    }
}