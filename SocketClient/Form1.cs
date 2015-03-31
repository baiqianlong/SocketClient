﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SocketClient
{
    public partial class Form1 : Form
    {
        string user = "";
        Socket cc = null;
        Thread clientThread = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             
        }

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="p_strMessage"></param>
        public void SendMessage(string p_strMessage)
        {
            byte[] bs = Encoding.ASCII.GetBytes(p_strMessage);   //把字符串编码为字节
            cc.Send(bs, bs.Length, 0); //发送信息
        }


        private void button1_Click(object sender, EventArgs e)
        {
             try
            {
                int port = int.Parse(txt_port.Text);
                string host = txt_ip.Text;
                //创建终结点EndPoint
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);   //把ip和端口转化为IPEndPoint的实例
                //创建Socket并连接到服务器
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   //  创建Socket
                cc = c;
                c.Connect(ipe); //连接到服务器
                clientThread = new Thread(new ThreadStart(ReceiveData));
                clientThread.Start();
                SetText("连接到服务器");
               
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("argumentNullException:{0}", ex);
            }
            catch (SocketException exp)
            {
                Console.WriteLine("SocketException:{0}",exp);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //向服务器发送信息
            string sendStr = user + ":" + txt_message.Text;
            byte[] bs = Encoding.UTF8.GetBytes(sendStr);   //把字符串编码为字节
            cc.Send(bs, bs.Length, 0); //发送信息
            rch_back.Text += "\n 我：" + sendStr;
            txt_message.Text = "";
        }

        public delegate void SetTextHandler(string text);
        private void SetText(string text)
        {
            if (rch_back.InvokeRequired == true)
            {
                SetTextHandler set = new SetTextHandler(SetText);//委托的方法参数应和SetText一致 
                rch_back.Invoke(set, new object[] { text }); //此方法第二参数用于传入方法,代替形参text 
            }
            else
            {
                rch_back.Text += text;
            }
        } 

        public void ReceiveData()
        {
            try
            {
                while (true)
                {
                    //接受从服务器返回的信息
                    string recvStr = "";
                    byte[] recvBytes = new byte[1024];
                    int bytes;
                    bytes = cc.Receive(recvBytes, recvBytes.Length, 0);    //从服务器端接受返回信息
                    recvStr = "\n Server:" + Encoding.UTF8.GetString(recvBytes, 0, bytes);
                    SetText(recvStr);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            cc.Shutdown(SocketShutdown.Both);
        }

        private void dgv_friend_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                user = dgv_friend[e.ColumnIndex, e.RowIndex].Value.ToString();
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
           
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            dgv_friend.Rows.Add(txt_user.Text);
        }
    }
}
