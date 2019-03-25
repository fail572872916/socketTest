using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace socket_server
{
    class timerClass
    {
        private  int invokeCount;
        private  int maxtime;           //最大延时时间
        private  Socket client;
        private string socket_flag;

        public timerClass(int count, Socket s, string socket_flag)
        {
            invokeCount = 0;
            maxtime = count;
            client = s;      
            this.socket_flag =  socket_flag;
        }

        // This method is called by the timer delegate.
        public  void CheckStatus(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;
          
            Console.WriteLine("{0} Checking status {1,2}.", DateTime.Now.ToString("h:mm:ss.fff"), (++invokeCount).ToString() + "~~~~~~~~~~线程号：" + Thread.CurrentThread.ManagedThreadId);
            // 在第7秒的时候发送心跳，10秒后接不到返回就释放对象

            if (invokeCount > 6&&invokeCount<8)
            {
                try
                {
                    byte[] heartBeat_package = new byte[1];
             
                  /*  string configToJosn = Newtonsoft.Json.JsonConvert.SerializeObject(send2json);  */ //转JOSN                                                                                                     //      server.onesend(server.clientFlag_list.Keys.ElementAt(i), configToJosn);
                    heartBeat_package = Encoding.UTF8.GetBytes("2");
                    client.Send(heartBeat_package, 0, heartBeat_package.Length, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Form1.f1.appendText(ex.Message + "计时器"+"\r\n");
                }
            }
            else if (invokeCount == maxtime)
            {
                try
                {
                    Console.WriteLine(socket_flag + "_________");
                    server.downLine(socket_flag);
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                    autoEvent.Set();
                }
                catch (Exception ex)
                {
                    Form1.f1.appendText(ex.Message+ "计时器1" + "\r\n");
                }
            }
        }
    }
}
