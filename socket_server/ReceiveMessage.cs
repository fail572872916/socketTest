using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace socket_server
{
    class ReceiveMessage
    {
        public System.Threading.Timer timer;
        byte[] buffer = new byte[128];
        string socket_flag;

        public ReceiveMessage(System.Threading.Timer s, byte[] buffer, string socket_flag)
        {
            timer = s;
            this.buffer = buffer;
            this.socket_flag = socket_flag;
            Console.WriteLine(this.socket_flag + "收到了消息");
        }
        public void OnReceiveCallback(IAsyncResult ar)
        {
            // timerClass statusChecker = new timerClass(3);
            try
            {
                Socket socket = ar.AsyncState as Socket;

                //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.endreceive.aspx
                int length = socket.EndReceive(ar);
                if (length > 0)
                {
                    timer.Dispose();

                    //读取出来消息内容
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    //    Form1.f1.appendText(message + "\r\n");

                    //   serialPort.WriteLine(message);  
                    // 解析JOSN,如果不是JOSN 数据就直接输出数据          
                    //if (josn.clientConfig(message) != null && message.Length > 1)
                    //{
                    //    config config = josn.clientConfig(message);
                    //    Form1.f1.appendText(message + "\r\n");
                    //    if (config.heartBeat == "0x0F")
                    //    {
                    //        Form1.f1.appendText(message + "\r\n");
                    //        timer.Dispose();
                    //    }
                    //    else if (config.operationCode == "onLine")               //判断上线操作
                    //    {
                    //        List<string> flaglist = new List<string>();
                    //        string configToJosn = null;
                    //        Form1.f1.appendText(socket.RemoteEndPoint.ToString());
                    //        socket_flag = config.flag;
                    //        if (server.clientFlag_list.Count == 0)
                    //        {
                    //            Form1.f1.addListbox_flag(config.flag);
                               
                    //            //添加flag到LIST  
                    //            flaglist.Add(socket_flag);
                    //            server.clientFlag_list.Add(config.flag, socket);   //将Flag 和 client 的套接定对应
                    //            config = config.addconfig(flaglist, socket_flag, null, null, "onLine", null);
                    //            configToJosn = Newtonsoft.Json.JsonConvert.SerializeObject(config);   //转JOSN      
                    //            server.allsend(configToJosn);
                    //        }
                    //        else
                    //        {
                    //            bool repetitive_key=false;
                    //            foreach (var item in server.clientFlag_list.Keys)
                    //            {
                                  
                    //                if (item == socket_flag)
                    //                {
                    //                     repetitive_key = true ;
                                      
                    //                    break;
                    //                    //Form1.f1.addListbox_flag(config.flag);
                    //                    //flaglist.Add(item);
                    //                    //config = config.addconfig(flaglist, item, "", null, "onLine", null);
                    //                    //configToJosn = Newtonsoft.Json.JsonConvert.SerializeObject(config);   //转JOSN      
                    //                    //server.allsend(configToJosn);
                                    
                    //                }
                    //            }
                    //            if (repetitive_key) 
                    //            {
                    //                Console.WriteLine("111111111111");
                    //                //    socket.Shutdown(SocketShutdown.Both);
                    //                socket.Close();
                    //                socket.Dispose();
                    //            }
                    //            else
                    //            {
                    //                Form1.f1.addListbox_flag(config.flag);

                    //                //添加flag到LIST  
                    //                flaglist.Add(socket_flag);
                    //                server.clientFlag_list.Add(config.flag, socket);   //将Flag 和 client 的套接定对应
                    //                config = config.addconfig(flaglist, socket_flag, null, null, "onLine", null);
                    //                configToJosn = Newtonsoft.Json.JsonConvert.SerializeObject(config);   //转JOSN      
                    //                server.allsend(configToJosn);
                    //            }
                    //        }
                    //    }
                    //    else if (config.operationCode == "P001" && config.message != "")
                    //    {
                    //        timer.Dispose();
                    //        Form1.f1.appendText(config.message);
                    //        //                analysisJosn(config.message);
                    //    }
                    //    else if (config.operationCode == "message" && config.message != "" && config.socket_ip == "")    //判断消息操作(回馈群发)
                    //    {

                    //        timer.Dispose();
                    //        Form1.f1.appendText(config.message + "\r\n");
                    //    }
                    //    else if (config.socket_ip != "" && config.message != "" && config.operationCode == "message")//判断消息操作(回馈单发)
                    //    {
                    //        timer.Dispose();
                    //        config send2json = config.addconfig(null, config.flag, null, config.message, "message", null);
                    //        string configToJosn = Newtonsoft.Json.JsonConvert.SerializeObject(send2json);   //转JOSN
                    //        server.onesend(config.socket_ip, configToJosn);
                    //    }
                    //    else
                    //    {
                    //        timer.Dispose();
                    //    }

                    ////}
                    //else
                    //{
                        Form1.f1.appendText("收到了消息:"+message + "\r\n");
                        //   server.allsend(message);
                    //}
                }
                // 判断client是否断开，没断开就继续调用自己，断开就删除这个连接的套接字
                if (server.IsSocketConnected(socket))
                {
                    StartKeepAlive StartKeepAlive = new StartKeepAlive(socket_flag,buffer);
                    StartKeepAlive._StartKeepAlive(socket);
                }
                else
                {
                    if (socket!=null)
                    {
                        server.downLine(socket_flag);
                    }
                    timer.Dispose();
                }
            }
            catch (Exception ex)
            {
                Form1.f1.appendText(ex.Message +"666"+ "\r\n");
            }
        }
    }


    public class StartKeepAlive
    {
        byte[] buffer = new byte[128];
        string socket_flag;

        public StartKeepAlive(string socket_flag, byte[] buffer)
        {       
            this.buffer = buffer;
            this.socket_flag = socket_flag;
            Console.WriteLine("这是 什么" + this.socket_flag);
        }

        /// <summary>
        /// 开始KeepAlive检测函数
        /// </summary>   
        public void _StartKeepAlive(Socket s)
        {
            var autoEvent = new AutoResetEvent(false);
            timerClass statusChecker = new timerClass(10, s, socket_flag);
            System.Threading.Timer stateTimer = new System.Threading.Timer(statusChecker.CheckStatus, autoEvent, 0, 1000);
            ReceiveMessage receiveMessage = new ReceiveMessage(stateTimer, buffer, socket_flag);
            s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveMessage.OnReceiveCallback), s);
            Thread.Sleep(10010);
            stateTimer.Dispose();
        }
    }
}
  
