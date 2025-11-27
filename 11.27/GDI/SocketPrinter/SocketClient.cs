using GDI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class SocketClient
{
    int i = 1;
   // Form1 show = new Form1();

    public enum State
    {
        Start, // 初始只有get没有1
        Wait,// 等待“get”
        Normal,//其余接收情况
    }
    State currentState = State.Start;

    private Socket client;
    private Thread rcvThread;
    public static bool isConnected = false;

    // 接收发送消息的显示委托给form1.cs里的textbox控件
    public Action<string> MsgFunc;
    public Action changeBtn;
    // ==============================================

    // 连接功能
    public void Connect(string ip, int port)
    {
        if (isConnected) return;

        Task.Run(() =>
        {
            try
            {
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                IPAddress IP = IPAddress.Parse(ip);      //获取设置的IP地址    
                IPEndPoint iPEndPoint = new IPEndPoint(IP, port);    //指定的端口号和服务器的ip建立一个IPEndPoint对象
                client.Connect(iPEndPoint);

                isConnected = true;

                
                MsgFunc?.Invoke(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "连接成功\r\n"); // 调用form1里的函数

                rcvThread = new Thread(ReceiveLoop);
                rcvThread.IsBackground = true;
                rcvThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("连接出错：" + ex.Message);
            }
        });
    }

    // 发送功能
    public void Send(string text)
    {
        if (client == null || !client.Connected)
        {
            MessageBox.Show("发送失败：未连接喷印系统");
            return;
        }

        try
        {
            if (!text.EndsWith("\r\n")) 
                text += "\r\n";

            byte[] data = Encoding.UTF8.GetBytes(text);
            client.Send(data);

          
            MsgFunc?.Invoke(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ")+ "发送：" + text); // 调用form1里的函数

        }
        catch (Exception ex)
        {
            MessageBox.Show("发送失败：" + ex.Message);
            Close();
        }
    }

    // 接收功能
    private void ReceiveLoop()
    {
        byte[] buffer = new byte[1024];

        while (isConnected)
        {
            try
            {
                int length = client.Receive(buffer);
                //int length = client.ReceiveAsync(buffer ); 有async版本，不使用多线程，而是用异步i/o实现高并发高效率的socket通信，以后再研究
               
                if (length == 0)
                {
                    MessageBox.Show("服务器断开连接");
                    changeBtn();
                    break;
                }

                string rcv = Encoding.UTF8.GetString(buffer, 0, length);

                if (MsgFunc != null)
                    MsgFunc(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "接收：" + rcv + "\r\n"); // 调用form1里的函数
                
                Console.WriteLine(rcv + "\r\n");

                // 加个函数把rcv发出去？如果rcv是"1"下一个是"get"，那么切换状态（状态是全局变量）

                // 获取文件数量
                if (!Directory.Exists(@"D:\img"))
                {
                    MessageBox.Show("不存在发送文件夹，socket接收线程结束");
                    return;
                }
                    
                int count = Directory.EnumerateFiles(@"D:\img", "*.bmp").Count();

                switch (currentState)
                {
                    case State.Start:
                        {
                            if (rcv == "get\r\n"|| ((rcv == "get\n")))
                            {        
                                if (i > count)
                                {
                                    i = 1;
                                    break;
                                }

                                // 获取文件名
                                string name = @"D:\img\" + i.ToString("D2") + ".bmp";
                                if (!name.EndsWith("\r\n"))
                                    name += "\r\n";


                                byte[] data = Encoding.UTF8.GetBytes(name);
                                //MessageBox.Show("发送送送送送送送");
                                // 发送文件路径

                                client.Send(data);
                                i++;

                                if (MsgFunc != null)
                                    MsgFunc(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "发送图片文件路径：" + name + "\r\n");


                            }          
                            currentState = State.Normal;

                            break;
                        }
                    case State.Normal:
                        {
                            if (rcv == "1\r\n" || ((rcv == "1\n")))
                                currentState = State.Wait;

                            break;
                        }
                    case State.Wait:
                        {
                            if(rcv == "get\r\n" || ((rcv == "get\n")))
                            {
                                if (i > count)
                                {
                                    i = 1;
                                    break;
                                }
                                    

                                // 获取文件名
                                string name = @"D:\img\" + i.ToString("D2") + ".bmp";
                                if (!name.EndsWith("\r\n"))
                                    name += "\r\n";
                                
                                byte[] data = Encoding.UTF8.GetBytes(name);
                                //MessageBox.Show("发送送送送送送送");
                                // 发送文件路径

                                client.Send(data);
                                i++;
                                if (MsgFunc != null)
                                    MsgFunc(DateTime.Now.ToString("yy-MM-dd hh:mm:ss ") + "发送图片文件路径：" + name + "\r\n");  
                            }
                            //currentState = State.Send;           

                            currentState = State.Normal;

                            break;
                        }
                }



            }
            catch(Exception ex)
            {
                if (isConnected) 
                    MessageBox.Show("连接异常断开:" + ex.ToString());

                break;
            }
        }
        Close();
    }

    public void Close()
    {
        isConnected = false;
        try 
        { 
            if (client != null) 
                client.Close();
        } 
        catch 
        {
            
        }
    }







}

