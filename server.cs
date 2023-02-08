using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace socket_server
{
    // ソケット通信（サーバー側）
    class server
    {
        static void Main()
        {
            // サーバーのIPアドレスとポート番号をセット
            IPAddress host1 = IPAddress.Parse("127.0.0.1");
            int port1 = 8765;
            IPEndPoint ipe1 = new IPEndPoint(host1, port1);

            TcpListener server = null;
            string recvline, sendline = null;
            int num, i = 0;
            Boolean outflg = false;
            // バイト配列のサイズを指定
            byte[] buf = new byte[1024];
            Regex reg = new Regex("\0");
            try
            {
                server = new TcpListener(ipe1);
                Console.WriteLine("クライアントからの入力待ち状態");
                server.Start();

                while(true)
                {
                    using (var client = server.AcceptTcpClient())
                    {
                        using (var stream = client.GetStream())
                        {
                            // クライアントから送信された文字列取得
                            while((i = stream.Read(buf, 0, buf.Length)) != 0)
                            {
                                // クライアントから受け取ったデータの後にnull文字(0x00)が入るのでReplaceで削除
                                recvline = reg.Replace(Encoding.UTF8.GetString(buf), "");

                                Console.WriteLine("client側の文字列：" + recvline);

                                // クライアントから"bye"の文字列取得でループ終了
                                if(recvline == "bye")
                                {
                                    outflg = true;
                                    break;
                                }

                                try
                                {
                                    // クライアントから受け取った数字を判定して返す文字列セット
                                    num = int.Parse(recvline);
                                    if(num % 2 == 0)
                                    {
                                        sendline = "OKです。";
                                    }
                                    else
                                    {
                                        sendline = "NGです。";
                                    }

                                }
                                catch
                                {
                                    sendline = "数値を入力してください。";
                                }
                                finally
                                {
                                    // セットした文字列をバイト配列にしてクライアントに送信
                                    buf = Encoding.UTF8.GetBytes(sendline);
                                    stream.Write(buf, 0, buf.Length);
                                    Array.Clear(buf, 0, buf.Length);
                                }

                            }
                        }
                    }
                    if(outflg == true)
                    {
                        break;
                    }
                }
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.Stop();
                Console.WriteLine("サーバー側終了です。");
            }
        }

    }
}
