using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace socket_server
{
     public partial class Form1 : Form
    {
        public static Form1 f1;
        server server = new server();
        public Form1()
        {
            InitializeComponent();
            f1 = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            asynServerRun _asynServerRun = new asynServerRun(server.runServer);
            _asynServerRun.BeginInvoke(null, null);
        } 

        private delegate void AppendText(string str);

        //委托添加文本
        private void appentTextBox1(string text)
        {
            if (textBox1.InvokeRequired)
            {
                textBox1.AppendText(text);
            }
            else
            {
                textBox1.AppendText(text);
            }
        }

        public void appendText(string msg)
        {
            AppendText aptxt = appentTextBox1;
            textBox1.BeginInvoke(aptxt, msg);
        }
        /// <summary>
        /// 对象发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[1024];
            buffer = Encoding.UTF8.GetBytes(txt_send.Text);      //将字符串转成byte数据       
            //判断就IP发送消息还是标签发送IP
            if (listBox1_socket.SelectedIndex != -1||listBox_flag.SelectedIndex!=-1)
            {
            
              if (listBox_flag.SelectedIndex != -1)
                {
                    try
                    {
                        server.clientFlag_list[listBox_flag.Text].Send(buffer, 0, buffer.Length, SocketFlags.None);
                    }
                    catch (Exception ex)
                    {
                        appentTextBox1(ex.Message + "\r\n");
                    }
                }               
            }
            else
            {
                MessageBox.Show("没有选中发送对象");
            }
        }

        /// <summary>
        /// 群发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_allsend_Click_1(object sender, EventArgs e)
        {
            server.allsend(txt_send.Text);
        }

        /// <summary>
        /// listbox_socket 添加
        /// </summary>
        /// <param name="str"></param>
        private delegate void addListItems(string str);
        private void addList(string text)
        {
            if (listBox1_socket.InvokeRequired)
            {
                listBox1_socket.Items.Add(text);
            }
            else
            {
                listBox1_socket.Items.Add(text);
            }
        }
        public void addItems(string msg)
        {
            addListItems _addListItems = addList;
            listBox1_socket.BeginInvoke(_addListItems, msg);
        }

        /// <summary>
        /// listbox_flag 添加
        /// </summary>
        /// <param name="str"></param>
        private delegate void addList_flag(string str);
        private void _addListbox_flag(string text)
        {
            if (listBox_flag.InvokeRequired)
            {
                listBox_flag.Items.Add(text);
            }
            else
            {
                listBox_flag.Items.Add(text);
            }
        }
        public void addListbox_flag(string msg)
        {
            addList_flag addList_flag = _addListbox_flag;
            listBox_flag.BeginInvoke(addList_flag, msg);
        }

        /// <summary>
        /// listbox_socket 删除
        /// </summary>
        /// <param name="str"></param>
        private delegate void RemoveListItems(string str);
        private void RemoveList(string text)
        {
            if (listBox1_socket.InvokeRequired)
            {
                listBox1_socket.Items.Remove(text);
            }
            else
            {
                listBox1_socket.Items.Remove(text);
            }
        }
        public void RemoveItems(string msg)
        {
            RemoveListItems RemoveListItems = RemoveList;
            listBox_flag.BeginInvoke(RemoveListItems, msg);
        }
        /// <summary>
        /// listbox_flag 删除
        /// </summary>
        /// <param name="str"></param>
        private delegate void RemoveList_flag(string str);
        private void _RemoveList_flag(string text)
        {
            if (listBox_flag.InvokeRequired)
            {
                listBox_flag.Items.Remove(text);
            }
            else
            {
                listBox_flag.Items.Remove(text);
            }
        }

        public void RemoveListbox_flag(string msg)
        {
            RemoveList_flag RemoveList_flag = _RemoveList_flag;
            listBox_flag.BeginInvoke(RemoveList_flag, msg);
        }

        //断开所有连接
        private void button3_Click(object sender, EventArgs e)
        {
            server.allClose();
        }
        //选取对象断开客户端的连接
        private void button4_Click(object sender, EventArgs e)
        {
            server.Close();
        }


        /// <summary>
        /// 轮洵给客户端发心跳（没了连接了就删除相应的服务端）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        byte[] heartBeat_package =  new byte[1];

        public delegate void HB (Socket s);    
        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (server.client_list.Count > 0)
            //{
            //    for (int i = 0; i < server.client_list.Keys.Count; i++)
            //    {
            //        try
            //        {
            //            server.client_list[server.client_list.Keys.ElementAt(i)].socket.Send(new byte[1], 0, 1, SocketFlags.None);
            //        }
            //        catch (Exception ex)
            //        {
            //            appentTextBox1(ex.Message + "\r\n");

            //            server.downLine(server.client_list.Keys.ElementAt(i));
            //        }
            //    }

        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show(server.clientFlag_list.Count.ToString());
            // server.clientFlag_list.Clear();
            //  timer1.Enabled = true;

        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }
    }
}